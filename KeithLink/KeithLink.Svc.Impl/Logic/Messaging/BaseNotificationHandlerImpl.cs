using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Messaging.Queue;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private IDsrServiceRepository dsrServiceRepository;
        #endregion

        #region ctor
        public BaseNotificationHandlerImpl(IUserProfileLogic userProfileLogic , IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository, 
                                                            IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory, IEventLogRepository log, 
                                                            IDsrServiceRepository dsrServiceRepository) {
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            this.messageProviderFactory = messageProviderFactory;
            this.log = log;
            this.dsrServiceRepository = dsrServiceRepository;
        }
        #endregion

        #region methods
        protected List<Recipient> LoadRecipients(NotificationType notificationType, Svc.Core.Models.Profile.Customer customer, bool dsrDSMOnly = false, bool getOnlyInternal = false, bool getOnlyExternal = false) {
            if (customer == null) { return new List<Recipient>(); }

            Svc.Core.Models.Profile.UserProfileReturn users = new Core.Models.Profile.UserProfileReturn();

            if (getOnlyInternal && !dsrDSMOnly && !getOnlyExternal)
            {
                //get all internal users and no external users
                users.UserProfiles.AddRange(userProfileLogic.GetInternalUsersWithAccessToCustomer(customer.CustomerNumber, customer.CustomerBranch)); //Retreive any internal users that have access to this customer
            }
            if (getOnlyExternal && !dsrDSMOnly && !getOnlyInternal)
            {
                //get all external users and no internal users
                users = userProfileLogic.GetUsers(new Core.Models.Profile.UserFilterModel() { CustomerId = customer.CustomerId });
            }
            if (dsrDSMOnly && !getOnlyExternal && !getOnlyInternal)
            {
                //Only load DSRs and DSMs for the customer

                //Load DSRs
                var dsr = dsrServiceRepository.GetDsr(customer.CustomerBranch, customer.DsrNumber);
                if (dsr != null && dsr.DsrNumber != "000" && !string.IsNullOrEmpty(dsr.EmailAddress)) {
                    users = (userProfileLogic.GetUserProfile(dsr.EmailAddress));
                }

                //Load DSM
                List<Core.Models.Profile.UserProfile> customerUsers = userProfileLogic.GetInternalUsersWithAccessToCustomer(customer.CustomerNumber, customer.CustomerBranch);
                Core.Models.Profile.UserProfile dsm = customerUsers.Where(x => x.DSMNumber == customer.DsmNumber).FirstOrDefault();
                if (dsm != null) {
                    users.UserProfiles.Add(dsm);
                }

            } else {
                users = userProfileLogic.GetUsers(new Core.Models.Profile.UserFilterModel() { CustomerId = customer.CustomerId });
                users.UserProfiles.AddRange(userProfileLogic.GetInternalUsersWithAccessToCustomer(customer.CustomerNumber, customer.CustomerBranch)); //Retreive any internal users that have access to this customer
            }

            List<UserMessagingPreference> userDefaultMessagingPreferences = // list of each user's default prefs
                userMessagingPreferenceRepository.ReadByUserIdsAndNotificationType(users.UserProfiles.Select(u => u.UserId), notificationType, true).ToList();
            List<UserMessagingPreference> customerMessagingPreferences = // list of customer's user specific pref
                userMessagingPreferenceRepository.ReadByCustomerAndNotificationType(customer.CustomerNumber, customer.CustomerBranch, notificationType).ToList();

            //foreach (Guid userId in customerMessagingPreferences.Select(x => x.UserId).Except(users.UserProfiles.Select(x => x.UserId)))
            //{
            //	// this will handle internal users that have a messaging pref setup for a customer
            //	users.UserProfiles.Add(userProfileLogic.GetUserProfile(userId).UserProfiles.FirstOrDefault());
            //}
            List<UserMessagingPreference> ump = new List<UserMessagingPreference>();
            ump.AddRange(userDefaultMessagingPreferences);
            ump.AddRange(customerMessagingPreferences);
            string prefs = string.Empty;

            foreach (var u in ump)
                prefs += u.Channel + u.UserId.ToString("B") + u.NotificationType;

            log.WriteInformationLog(String.Format("notification prefs: {0}, profiles count: {1}, userDefaultMessagingPreferences: {2}, customerMessagingPreferences: {3}",
                                                   prefs, users.UserProfiles.Count, userDefaultMessagingPreferences, customerMessagingPreferences));

            List<Recipient> recipients = new List<Recipient>();

            foreach (Svc.Core.Models.Profile.UserProfile userProfile in users.UserProfiles) {
                if (userDefaultMessagingPreferences != null) {
                    // first, check for customer specific prefs
                    IEnumerable<UserMessagingPreference> prefsToUse = customerMessagingPreferences.Where(
                        p => p.UserId == userProfile.UserId).ToList(); // check for customer specific prefs first
                    if (prefsToUse == null || prefsToUse.Count() == 0) // then check for defaults
                        prefsToUse = userDefaultMessagingPreferences.Where(p => p.UserId == userProfile.UserId);

                    foreach (var pref in prefsToUse) {
                        if (pref.Channel == Channel.Email) {
                            recipients.Add(new Recipient() { ProviderEndpoint = userProfile.EmailAddress, Channel = Channel.Email });
                        } else if (pref.Channel == Channel.MobilePush) {
                            // lookup any and all mobile devices
                            foreach (var device in userPushNotificationDeviceRepository.ReadUserDevices(userProfile.UserId))
                                recipients.Add(new Recipient() { ProviderEndpoint = device.ProviderEndpointId, DeviceOS = device.DeviceOS, Channel = Channel.MobilePush });
                        } else if (pref.Channel == Channel.Web) {
                            recipients.Add(new Recipient() { UserId = userProfile.UserId, CustomerNumber = customer.CustomerNumber, Channel = Channel.Web });
                        }
                    }
                }
            }

            return recipients;
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
