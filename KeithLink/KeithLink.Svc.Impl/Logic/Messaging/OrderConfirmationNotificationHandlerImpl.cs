using System;
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
    public class OrderConfirmationNotificationHandlerImpl : INotificationHandler
    {
        #region attributes
        IEventLogRepository eventLogRepository;
        IUserProfileLogic userProfileLogic;
        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        ICustomerRepository customerRepository;
        #endregion
        public OrderConfirmationNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic
            , IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository)
        {
            this.eventLogRepository = eventLogRepository;
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
        }

        public void ProcessNotification(Core.Models.Messaging.Queue.BaseNotification notification)
        {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.OrderConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            OrderConfirmationNotification orderConfirmation = (OrderConfirmationNotification)notification;
            // create different formatted messages for each channel - do email and push for now
            Message emailMessage = GetEmailMessageForNotification(orderConfirmation);
            Message pushMessage = GetEmailMessageForNotification(orderConfirmation);
            Message webMessage = GetEmailMessageForNotification(orderConfirmation);

            // load up recipients by message type
            List<Recipient> emailRecipients = new List<Recipient>();
            List<Recipient> webRecipients = new List<Recipient>();
            List<Recipient> pushRecipients = new List<Recipient>();
            LoadRecipients(orderConfirmation, emailRecipients, webRecipients, pushRecipients);

            // send messages to providers...
        }

        private void LoadRecipients(OrderConfirmationNotification orderConfirmation, List<Recipient> emailRecipients, List<Recipient> webRecipients, List<Recipient> pushRecipients)
        {
            Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(orderConfirmation.CustomerNumber);
            Svc.Core.Models.Profile.UserProfileReturn users = userProfileLogic.GetUsers(new Core.Models.Profile.UserFilterModel() { CustomerId = customer.CustomerId });

            foreach (Svc.Core.Models.Profile.UserProfile userProfile in users.UserProfiles)
            {
                // look for customer prefs; otherwise look for default prefs
                IEnumerable<Core.Models.Messaging.ProfileMessagingPreferenceDetailModel> profileMessagingPreferences = LoadMessagingPreferences(orderConfirmation, userProfile);

                if (profileMessagingPreferences != null)
                {
                    foreach (var prefs in profileMessagingPreferences)
                    {
                        foreach (var c in prefs.SelectedChannels)
                        {
                            if (c.Channel == Channel.Email)
                            {
                                emailRecipients.Add(new Recipient() { ProviderEndpoint = userProfile.EmailAddress });
                            }
                            else if (c.Channel == Channel.MobilePush)
                            {
                                // lookup any and all mobile devices
                                foreach (var device in userPushNotificationDeviceRepository.ReadUserDevices(userProfile.UserId))
                                    pushRecipients.Add(new Recipient() { ProviderEndpoint = device.ProviderEndpointId });
                            }
                            else if (c.Channel == Channel.Web)
                            {
                                webRecipients.Add(new Recipient() { UserId = userProfile.UserId, CustomerNumber = orderConfirmation.CustomerNumber });
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<Core.Models.Messaging.ProfileMessagingPreferenceDetailModel> LoadMessagingPreferences(OrderConfirmationNotification orderConfirmation, Svc.Core.Models.Profile.UserProfile userProfile)
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

        private Message GetEmailMessageForNotification(OrderConfirmationNotification notification)
        { // TODO: plugin message templates so some of this text can come from the database
            string statusString = String.IsNullOrEmpty(notification.OrderChange.OriginalStatus)
                ? "Order confirmed with status: " + notification.OrderChange.CurrentStatus
                : "Order updated from status: " + notification.OrderChange.OriginalStatus + " to " + notification.OrderChange.CurrentStatus;

            string orderLineChanges = string.Empty;
            foreach (var line in notification.OrderChange.ItemChanges)
                orderLineChanges += orderLineChanges + "Item: " + line.ItemNumber +
                    (String.IsNullOrEmpty(line.SubstitutedItemNumber) ? string.Empty : ("replace by: " + line.SubstitutedItemNumber)) +
                    "  Status: " + line.NewStatus + (line.NewStatus == line.OriginalStatus || string.IsNullOrEmpty(line.OriginalStatus)
                                                        ? string.Empty : (" change from: " + line.OriginalStatus)) + System.Environment.NewLine;

            string originalOrderInfo = "Original Order Information:" + System.Environment.NewLine;
            foreach (var line in notification.OrderChange.Items)
                originalOrderInfo += line.ItemNumber + ", " + line.ItemDescription + " (" + line.QuantityOrdered + ")" + System.Environment.NewLine;

            Message message = new Message();
            message.MessageSubject = "BEK: Order Confirmation for " + notification.CustomerNumber + " (" + notification.OrderChange.OrderName + ")";
            message.MessageBody = statusString + System.Environment.NewLine + orderLineChanges + System.Environment.NewLine + originalOrderInfo;
            return message;
        }

        private IMessageProvider GetMessageProvider(Channel channel)
        {
            throw new NotImplementedException();
        }
    }
}
