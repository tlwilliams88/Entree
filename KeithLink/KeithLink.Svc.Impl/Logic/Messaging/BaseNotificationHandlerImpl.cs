using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;

using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Profile;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging {
    public abstract class BaseNotificationHandlerImpl {
        #region attributes
        private IUserProfileLogic userProfileLogic;
        private IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        private ICustomerRepository customerRepository;
        private IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        private Func<Channel, IMessageProvider> messageProviderFactory;
        private IEventLogRepository log;
        private IDsrLogic _dsrLogic;
        #endregion

        #region ctor
        public BaseNotificationHandlerImpl(IUserProfileLogic userProfileLogic , IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository, 
                                                            IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory, IEventLogRepository log, 
                                                            IDsrLogic dsrLogic) {
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            this.messageProviderFactory = messageProviderFactory;
            this.log = log;
            _dsrLogic = dsrLogic;
        }
        #endregion

        #region methods
        private UserProfileReturn GetUsers(Customer customer, bool dsrDSMOnly) {
            UserProfileReturn users = new UserProfileReturn();

            if(dsrDSMOnly) {
                //Only load DSRs and DSMs for the customer

                //Load DSRs
                var dsr = _dsrLogic.GetDsr(customer.CustomerBranch, customer.DsrNumber);
                if(dsr != null && dsr.DsrNumber != "000" && !string.IsNullOrEmpty(dsr.EmailAddress)) {
                    users = (userProfileLogic.GetUserProfile(dsr.EmailAddress, createBekProfile: false));
                }

                //Load DSM
                List<UserProfile> customerUsers = userProfileLogic.GetInternalUsersWithAccessToCustomer(customer.CustomerNumber, customer.CustomerBranch);

                if(customerUsers == null || customerUsers.Count == 0) {
                    // no internal users found with access to the customer
                    log.WriteWarningLog(string.Format("Could not find any internal users with access to {0}-{1}", customer.CustomerBranch, customer.CustomerNumber));
                } else {
                    UserProfile dsm = customerUsers.Where(x =>  x.IsDSM &&
                                                                !string.IsNullOrEmpty(x.DSMNumber) && 
                                                                x.DSMNumber == customer.DsmNumber)
                                                   .FirstOrDefault();
                    
                    if(dsm != null) {
                        users.UserProfiles.Add(dsm);
                    }
                }
            } else {
                users = userProfileLogic.GetUsers(new UserFilterModel() { CustomerId = customer.CustomerId });
                users.UserProfiles.AddRange(userProfileLogic.GetInternalUsersWithAccessToCustomer(customer.CustomerNumber, customer.CustomerBranch)); //Retreive any internal users that have access to this customer
            }

            // make sure that we return good data in the users list
            if(users == null || users.UserProfiles == null) {
                users =  new UserProfileReturn();
            } else {
                users.UserProfiles.RemoveAll(u => u == null || u.UserId == null);
            }

            return users;
        }

        protected List<Recipient> LoadRecipients(NotificationType notificationType, Customer customer, bool dsrDSMOnly = false) {
            if (customer == null) { return new List<Recipient>(); }

            UserProfileReturn users = GetUsers(customer, dsrDSMOnly);

            if(users.UserProfiles.Count == 0) { return new List<Recipient>(); }


            List<UserMessagingPreference> userDefaultMessagingPreferences = // list of each user's default prefs
                userMessagingPreferenceRepository.ReadByUserIdsAndNotificationType(users.UserProfiles.Select(u => u.UserId), notificationType, true).ToList();
            List<UserMessagingPreference> customerMessagingPreferences = // list of customer's user specific pref
                userMessagingPreferenceRepository.ReadByCustomerAndNotificationType(customer.CustomerNumber, customer.CustomerBranch, notificationType).ToList();

            List<UserMessagingPreference> ump = new List<UserMessagingPreference>();
            ump.AddRange(userDefaultMessagingPreferences);
            ump.AddRange(customerMessagingPreferences);
            string prefs = string.Empty;

            foreach (var u in ump)
                prefs += u.Channel + u.UserId.ToString("B") + u.NotificationType;

            log.WriteInformationLog(String.Format("notification prefs: {0}, profiles count: {1}, profiles: {2}, userDefaultMessagingPreferences: {3}, customerMessagingPreferences: {4}",
                                                   prefs, users.UserProfiles.Count, JsonConvert.SerializeObject(users.UserProfiles), userDefaultMessagingPreferences, customerMessagingPreferences));

            List<Recipient> recipients = new List<Recipient>();

            foreach(UserProfile userProfile in users.UserProfiles) {
                if(userDefaultMessagingPreferences != null) {
                    // first, check for customer specific prefs
                    List<UserMessagingPreference> prefsToUse = customerMessagingPreferences.Where(
                        p => p.UserId == userProfile.UserId).ToList(); // check for customer specific prefs first
                    if(prefsToUse == null || prefsToUse.Count() == 0) // then check for defaults
                        prefsToUse = userDefaultMessagingPreferences.Where(p => p.UserId == userProfile.UserId).ToList();

                    foreach(var pref in prefsToUse) {
                        if(pref.Channel == Channel.Email) {
                            recipients.Add(new Recipient() { ProviderEndpoint = userProfile.EmailAddress,
                                                             Channel = Channel.Email,
                                                             UserId = userProfile.UserId,
                                                             UserEmail = userProfile.EmailAddress,
                                                             CustomerNumber = customer.CustomerNumber});
                        } else if(pref.Channel == Channel.MobilePush) {
                            // lookup any and all mobile devices
                            foreach (var device in userPushNotificationDeviceRepository.ReadUserDevices(userProfile.UserId))
                                if (device.Enabled != false)
                                {
                                    recipients.Add(new Recipient()
                                    {
                                        ProviderEndpoint = device.ProviderEndpointId,
                                        DeviceOS = device.DeviceOS,
                                        Channel = Channel.MobilePush,
                                        UserId = userProfile.UserId,
                                        UserEmail = userProfile.EmailAddress,
                                        DeviceId = device.DeviceId,
                                        CustomerNumber = customer.CustomerNumber
                                    });
                                }
                        } else if(pref.Channel == Channel.Web) {
                            recipients.Add(new Recipient() { UserId = userProfile.UserId,
                                                             CustomerNumber = customer.CustomerNumber,
                                                             Channel = Channel.Web,
                                                             UserEmail = userProfile.EmailAddress});
                        }
                    }
                }
            }

            Dictionary<string, Recipient> dict = new Dictionary<string, Recipient>();
            foreach (Recipient rec in recipients)
            {
                string dupkey = rec.UserId + "_" + rec.CustomerNumber + "_" + rec.Channel + "_" + rec.ProviderEndpoint;
                if (dict.Keys.Contains(dupkey, StringComparer.CurrentCultureIgnoreCase) == false)
                {
                    dict.Add(dupkey, rec);
                }
            }

            return dict.Values.ToList();
        }

        protected void SendMessage(List<Recipient> recipients, Message message) {
            // TODO: Turn this into one line of code that doesn't depend on specific channels...
            messageProviderFactory(Channel.Email).SendMessage(recipients.Where(r => r.Channel == Channel.Email).ToList(), message);
            messageProviderFactory(Channel.MobilePush).SendMessage(recipients.Where(r => r.Channel == Channel.MobilePush).ToList(), message);
            messageProviderFactory(Channel.Web).SendMessage(recipients.Where(r => r.Channel == Channel.Web).ToList(), message);
        }
        #endregion
    }
}
