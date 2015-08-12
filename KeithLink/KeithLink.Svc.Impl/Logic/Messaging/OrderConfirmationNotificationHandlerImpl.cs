using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class OrderConfirmationNotificationHandlerImpl : BaseNotificationHandlerImpl, INotificationHandler
    {
        #region attributes
        ICatalogRepository _catRepo;
        IEventLogRepository eventLogRepository;
        IUserProfileLogic userProfileLogic;
        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        ICustomerRepository customerRepository;
        IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        Func<Channel, IMessageProvider> messageProviderFactory;
		private readonly IDsrServiceRepository dsrServiceRepository;
        #endregion

        #region ctor
        public OrderConfirmationNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic, IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, 
                                                                                ICustomerRepository customerRepository , IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory, 
                                                                                IDsrServiceRepository dsrServiceRepository, ICatalogRepository catalogRepository )
            : base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository,
                     userMessagingPreferenceRepository, messageProviderFactory, eventLogRepository, 
                     dsrServiceRepository)
        {
            _catRepo = catalogRepository;
            this.eventLogRepository = eventLogRepository;
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            this.messageProviderFactory = messageProviderFactory;
        }
        #endregion

        #region methods
        public void ProcessNotification(Core.Models.Messaging.Queue.BaseNotification notification)
        {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.OrderConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            OrderConfirmationNotification orderConfirmation = (OrderConfirmationNotification)notification;

            // load up recipients, customer and message
            eventLogRepository.WriteInformationLog("order confirmation, custNum: " + notification.CustomerNumber + ", branch: " + notification.BranchId);
            Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber, notification.BranchId);

            if (customer == null) {
                System.Text.StringBuilder warningMessage = new StringBuilder();
                warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Order Confirmation notification.", notification.BranchId, notification.CustomerNumber);
                warningMessage.AppendLine();
                warningMessage.AppendLine();
                warningMessage.AppendLine("Notification:");
                warningMessage.AppendLine(notification.ToJson());

                eventLogRepository.WriteWarningLog(warningMessage.ToString());
            } else {
                List<Recipient> recipients = base.LoadRecipients(orderConfirmation.NotificationType, customer);
                Message message = GetEmailMessageForNotification(orderConfirmation, customer);

                // send messages to providers...
                if (recipients != null && recipients.Count > 0) {
                    base.SendMessage(recipients, message);
                }
            }
        }

        private Message GetEmailMessageForNotification(OrderConfirmationNotification notification, Svc.Core.Models.Profile.Customer customer)
        { // TODO: plugin message templates so some of this text can come from the database
            string statusString = String.IsNullOrEmpty(notification.OrderChange.OriginalStatus)
                ? notification.OrderChange.CurrentStatus.Equals("rejected", StringComparison.CurrentCultureIgnoreCase) ? "Order Rejected: " + notification.OrderChange.SpecialInstructions :  "Order confirmed with status: " + notification.OrderChange.CurrentStatus
                : "Order updated from status: " + notification.OrderChange.OriginalStatus + " to " + notification.OrderChange.CurrentStatus;
			
            string orderLineChanges = string.Empty;
            foreach (var line in notification.OrderChange.ItemChanges)
                orderLineChanges += orderLineChanges + "Item: " + line.ItemNumber +
                    (String.IsNullOrEmpty(line.SubstitutedItemNumber) ? string.Empty : ("replace by: " + line.SubstitutedItemNumber)) +
                    "  Status: " + line.NewStatus + (line.NewStatus == line.OriginalStatus || string.IsNullOrEmpty(line.OriginalStatus)
                                                        ? string.Empty : (" change from: " + line.OriginalStatus)) + System.Environment.NewLine;

            ProductsReturn products = _catRepo.GetProductsByIds(customer.CustomerBranch, notification.OrderChange.Items.Select(i => i.ItemNumber).ToList());

            string originalOrderInfo = "Original Order Information:" + System.Environment.NewLine;
            foreach (var line in notification.OrderChange.Items) {
                Product currentProduct = products.Products.Where(i => i.ItemNumber == line.ItemNumber).FirstOrDefault();

                string[] args = new string[]{
                                                line.ItemNumber,
                                                line.ItemDescription,
                                                line.QuantityOrdered.ToString(),
                                                line.ItemPrice.ToString("f2"),
                                                currentProduct.CatchWeight ? (line.Each ? "lb per package" : "lb per case") : (line.Each ? "package" : "case"),
                                                System.Environment.NewLine
                                            };
                originalOrderInfo += string.Format("{0} - {1} (Quantity Ordered: {2} ; Price: ${3} per {4}){5}", args);
            }

            Message message = new Message();

			if (!string.IsNullOrEmpty(notification.OrderChange.CurrentStatus) && notification.OrderChange.CurrentStatus.Equals("rejected", StringComparison.CurrentCultureIgnoreCase))
				message.MessageSubject = "Ben E. Keith: Order Rejected for " + string.Format("{0}-{1}", customer.CustomerNumber, customer.CustomerName) + " (" + notification.OrderChange.OrderName + ")";
			else
				message.MessageSubject = "Ben E. Keith: Order Confirmation for " + string.Format("{0}-{1}", customer.CustomerNumber, customer.CustomerName) + " (" + notification.OrderChange.OrderName + ")";

            message.MessageBody = (!string.IsNullOrEmpty(notification.OrderChange.SpecialInstructions) ? "Instructions: " + notification.OrderChange.SpecialInstructions + System.Environment.NewLine : "") +
                (notification.OrderChange.ShipDate > DateTime.MinValue ? "Ship Date: " + notification.OrderChange.ShipDate.ToShortDateString() + System.Environment.NewLine + System.Environment.NewLine : "") +
				statusString + System.Environment.NewLine + 
				orderLineChanges + System.Environment.NewLine + 
				originalOrderInfo;
            message.CustomerNumber = customer.CustomerNumber;
            message.CustomerName = customer.CustomerName;
			message.BranchId = customer.CustomerBranch;
			message.NotificationType = NotificationType.OrderConfirmation;
            return message;
        }
        #endregion
    }
}
