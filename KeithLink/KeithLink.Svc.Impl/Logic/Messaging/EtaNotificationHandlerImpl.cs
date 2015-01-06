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
    public class EtaNotificationHandlerImpl : BaseNotificationHandlerImpl, INotificationHandler
    {
        #region attributes
        IEventLogRepository eventLogRepository;
        IUserProfileLogic userProfileLogic;
        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        ICustomerRepository customerRepository;
        IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        Func<Channel, IMessageProvider> messageProviderFactory;
        #endregion
        public EtaNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic
            , IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository
            , IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory)
            : base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository
                    , userMessagingPreferenceRepository, messageProviderFactory)
        {
            this.eventLogRepository = eventLogRepository;
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            this.messageProviderFactory = messageProviderFactory;
        }

        public void ProcessNotification(Core.Models.Messaging.Queue.BaseNotification notification)
        {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.OrderConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            EtaNotification eta = (EtaNotification)notification;

            // load up recipients, customer and message
            // get all orders, then get order details, then get distinct customer list then get recipient by customer, etc ...
            eventLogRepository.WriteErrorLog("Received ETA Message: " + eta.ToString());

            //Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber);
            //List<Recipient> recipients = base.LoadRecipients(orderConfirmation.NotificationType, customer);
            //Message message = GetEmailMessageForNotification(orderConfirmation, customer);

            // send messages to providers...
            //base.SendMessage(recipients, message);
        }

        private Message GetEmailMessageForNotification(OrderConfirmationNotification notification, Svc.Core.Models.Profile.Customer customer)
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
            message.CustomerNumber = customer.CustomerNumber;
            message.CustomerNumber = customer.CustomerNumber;
			message.NotificationType = NotificationType.OrderConfirmation;
            return message;
        }
    }
}
