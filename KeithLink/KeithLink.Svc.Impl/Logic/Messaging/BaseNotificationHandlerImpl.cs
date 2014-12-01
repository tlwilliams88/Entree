﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Profile;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public abstract class BaseNotificationHandlerImpl
    {
        #region attributes
        IUserProfileLogic userProfileLogic;
        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        ICustomerRepository customerRepository;
        IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        Func<Channel, IMessageProvider> messageProviderFactory;
        #endregion

        #region ctor
        public BaseNotificationHandlerImpl(IUserProfileLogic userProfileLogic
            , IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository
            , IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory)
        {
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            this.messageProviderFactory = messageProviderFactory;
        }
        #endregion

        protected List<Recipient> LoadRecipients(NotificationType notificationType, Svc.Core.Models.Profile.Customer customer)
        {
            Svc.Core.Models.Profile.UserProfileReturn users = userProfileLogic.GetUsers(new Core.Models.Profile.UserFilterModel() { CustomerId = customer.CustomerId });
            List<UserMessagingPreference> profileMessagingPreferences =
                userMessagingPreferenceRepository.ReadByUserIdsAndNotificationType(users.UserProfiles.Select(u => u.UserId), notificationType).ToList();
            List<Recipient> recipients = new List<Recipient>();

            foreach (Svc.Core.Models.Profile.UserProfile userProfile in users.UserProfiles)
            {
                if (profileMessagingPreferences != null)
                {
                    // first, check for customer specific prefs
                    IEnumerable<UserMessagingPreference> prefsToUse = profileMessagingPreferences.Where(
                        p => p.CustomerNumber == customer.CustomerNumber);
                    if (prefsToUse == null || prefsToUse.Count() == 0)
                        prefsToUse = profileMessagingPreferences.Where(p => p.CustomerNumber == null);
                    
                    foreach (var pref in prefsToUse)
                    {
                        if (pref.Channel == Channel.Email)
                        {
                            recipients.Add(new Recipient() { ProviderEndpoint = userProfile.EmailAddress, Channel = Channel.Email });
                        }
                        else if (pref.Channel == Channel.MobilePush)
                        {
                            // lookup any and all mobile devices
                            foreach (var device in userPushNotificationDeviceRepository.ReadUserDevices(userProfile.UserId))
                                recipients.Add(new Recipient() { ProviderEndpoint = device.ProviderEndpointId, DeviceOS = device.DeviceOS, Channel = Channel.MobilePush });
                        }
                        else if (pref.Channel == Channel.Web)
                        {
                            recipients.Add(new Recipient() { UserId = userProfile.UserId, CustomerNumber = customer.CustomerNumber, Channel = Channel.Web });
                        }
                    }
                }
            }

            return recipients;
        }

        protected static IEnumerable<Core.Models.Messaging.ProfileMessagingPreferenceDetailModel> LoadMessagingPreferences(OrderConfirmationNotification orderConfirmation, Svc.Core.Models.Profile.UserProfile userProfile)
        {
            IEnumerable<Core.Models.Messaging.ProfileMessagingPreferenceDetailModel> profileMessagingPreferences = null;
            if (userProfile.MessagingPreferences != null && userProfile.MessagingPreferences.Any(
                p => p.CustomerNumber == orderConfirmation.CustomerNumber))
            {
                profileMessagingPreferences =
                    userProfile.MessagingPreferences.Where(p => p.CustomerNumber == orderConfirmation.CustomerNumber).FirstOrDefault()
                        .Preferences.Where(p => p.NotificationType == orderConfirmation.NotificationType);
            }
            else if (userProfile.MessagingPreferences != null && userProfile.MessagingPreferences.Any(
                p => p.CustomerNumber == null))
            {
                profileMessagingPreferences =
                    userProfile.MessagingPreferences.Where(p => p.CustomerNumber == null).FirstOrDefault()
                        .Preferences.Where(p => p.NotificationType == orderConfirmation.NotificationType);
            }
            return profileMessagingPreferences;
        }

        protected void SendMessage(List<Recipient> recipients, Message message)
        {
            // TODO: Turn this into one line of code that doesn't depend on specific channels...
            messageProviderFactory(Channel.Email).SendMessage(recipients.Where(r => r.Channel == Channel.Email), message);
            messageProviderFactory(Channel.MobilePush).SendMessage(recipients.Where(r => r.Channel == Channel.MobilePush), message);
            messageProviderFactory(Channel.Web).SendMessage(recipients.Where(r => r.Channel == Channel.Web), message);
        }
    }
}
