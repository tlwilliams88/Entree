using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Orders;
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
        private const string MESSAGE_TEMPLATE_ORDERCONFIRMATION = "OrderConfirmation";
        private const string MESSAGE_TEMPLATE_ORDERSUCCESSFUL = "OrderSuccessful";
        private const string MESSAGE_TEMPLATE_ORDERREJECTED = "OrderRejected";
        private const string MESSAGE_TEMPLATE_ORDERCHANGE = "OrderChange";
        private const string MESSAGE_TEMPLATE_ORDERCHANGEDETAIL = "OrderChangeDetail";
        private const string MESSAGE_TEMPLATE_ORDERITEMS = "OrderConfirmationItems";
        private const string MESSAGE_TEMPLATE_ORDERITEMDETAIL = "OrderConfirmationItemDetail";
        private const string MESSAGE_TEMPLATE_ORDERITEMSOOS = "OrderConfirmationItemsOOS";
        private const string MESSAGE_TEMPLATE_ORDERITEMOOSDETAIL = "OrderConfirmationItemOOSDetail";
        ICatalogRepository _catRepo;
        IEventLogRepository eventLogRepository;
        IUserProfileLogic userProfileLogic;
        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        ICustomerRepository customerRepository;
        private readonly IMessageTemplateLogic _messageTemplateLogic;
        IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        Func<Channel, IMessageProvider> messageProviderFactory;
		private readonly IDsrServiceRepository dsrServiceRepository;
        private IInternalOrderHistoryLogic _orderHistoryLogic;
        #endregion

        #region ctor
        public OrderConfirmationNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic, 
                                                        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository,
                                                        IMessageTemplateLogic messageTemplateLogic, ICustomerRepository customerRepository, 
                                                        IUserMessagingPreferenceRepository userMessagingPreferenceRepository, 
                                                        Func<Channel, IMessageProvider> messageProviderFactory, 
                                                        IDsrServiceRepository dsrServiceRepository, ICatalogRepository catalogRepository,
                                                        IInternalOrderHistoryLogic orderHistoryLogic)
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
            _messageTemplateLogic = messageTemplateLogic;
            this.messageProviderFactory = messageProviderFactory;
            this._orderHistoryLogic = orderHistoryLogic;
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


        public void ProcessNotificationForExternalUsers(Core.Models.Messaging.Queue.BaseNotification notification)
        {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.OrderConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            OrderConfirmationNotification orderConfirmation = (OrderConfirmationNotification)notification;

            // load up recipients, customer and message
            eventLogRepository.WriteInformationLog("order confirmation, custNum: " + notification.CustomerNumber + ", branch: " + notification.BranchId);
            Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber, notification.BranchId);

            if (customer == null)
            {
                System.Text.StringBuilder warningMessage = new StringBuilder();
                warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Order Confirmation notification.", notification.BranchId, notification.CustomerNumber);
                warningMessage.AppendLine();
                warningMessage.AppendLine();
                warningMessage.AppendLine("Notification:");
                warningMessage.AppendLine(notification.ToJson());

                eventLogRepository.WriteWarningLog(warningMessage.ToString());
            }
            else
            {
                List<Recipient> recipients = base.LoadRecipients(orderConfirmation.NotificationType, customer, false, false, true); 
                Message message = GetEmailMessageForNotification(orderConfirmation, customer);

                // send messages to providers...
                if (recipients != null && recipients.Count > 0)
                {
                    try
                    {
                        base.SendMessage(recipients, message);
                    }
                    catch (Exception ex)
                    {
                        eventLogRepository.WriteErrorLog(String.Format("Error sending messages {0} {1}", ex.Message, ex.StackTrace));
                    } 
                }
            }
        }

        public void ProcessNotificationForInternalUsers(Core.Models.Messaging.Queue.BaseNotification notification)
        {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.OrderConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            OrderConfirmationNotification orderConfirmation = (OrderConfirmationNotification)notification;

            // load up recipients, customer and message
            eventLogRepository.WriteInformationLog("order confirmation, custNum: " + notification.CustomerNumber + ", branch: " + notification.BranchId);
            Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber, notification.BranchId);

            if (customer == null)
            {
                System.Text.StringBuilder warningMessage = new StringBuilder();
                warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Order Confirmation notification.", notification.BranchId, notification.CustomerNumber);
                warningMessage.AppendLine();
                warningMessage.AppendLine();
                warningMessage.AppendLine("Notification:");
                warningMessage.AppendLine(notification.ToJson());

                eventLogRepository.WriteWarningLog(warningMessage.ToString());
            }
            else
            {
                List<Recipient> recipients = base.LoadRecipients(orderConfirmation.NotificationType, customer,false,true,false);
                Message message = GetEmailMessageForNotification(orderConfirmation, customer);

                // send messages to providers...
                if (recipients != null && recipients.Count > 0)
                {
                    try
                    {
                        base.SendMessage(recipients, message);
                    }
                    catch (Exception ex)
                    {
                        eventLogRepository.WriteErrorLog(String.Format("Error sending messages {0} {1}", ex.Message, ex.StackTrace));
                    } 
                }
            }
        }

        private Message GetEmailMessageForNotification(OrderConfirmationNotification notification, Svc.Core.Models.Profile.Customer customer)
        {
            StringBuilder statusString = new StringBuilder();
            if (notification.OrderChange.CurrentStatus.Equals("rejected", StringComparison.CurrentCultureIgnoreCase))
            {
                return MakeRejectedMessage(notification, customer);
            }
            StringBuilder originalOrderInfo = new StringBuilder();
            decimal totalAmount = BuildOrderTable(notification, customer, originalOrderInfo);
            return MakeConfirmationMessage(notification, customer, originalOrderInfo, totalAmount);
        }

        private Message MakeConfirmationMessage(OrderConfirmationNotification notification, Svc.Core.Models.Profile.Customer customer, StringBuilder originalOrderInfo, decimal totalAmount)
        {
            string invoiceNumber = GetInvoiceNumber(notification, customer);
            Message message = new Message();
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERCONFIRMATION);
            message.MessageSubject = template.Subject.Inject(new
            {
                OrderStatus = "Order Confirmation",
                CustomerNumber = customer.CustomerNumber,
                CustomerName = customer.CustomerName,
                InvoiceNumber = invoiceNumber
            });
            StringBuilder sbShipDate = new StringBuilder();
            if ((notification != null) && 
                (notification.OrderChange != null) && 
                (notification.OrderChange.ShipDate != null) &&
                (notification.OrderChange.ShipDate.Equals("1/1/0001") == false))
                sbShipDate.Append(notification.OrderChange.ShipDate);
            else sbShipDate.Append("Undetermined");
            message.MessageBody = template.Body.Inject(new
            {
                CustomerNumber = customer.CustomerNumber,
                CustomerName = customer.CustomerName,
                InvoiceNumber = invoiceNumber,
                ShipDate = sbShipDate.ToString(),
                Count = notification.OrderChange.Items.Count,
                Total = totalAmount.ToString("f2"),
                PurchaseOrder = notification.OrderChange.OrderName,
                OrderConfirmationItems = originalOrderInfo.ToString()
            });
            message.BodyIsHtml = template.IsBodyHtml;
            message.CustomerNumber = customer.CustomerNumber;
            message.CustomerName = customer.CustomerName;
            message.BranchId = customer.CustomerBranch;
            message.NotificationType = NotificationType.OrderConfirmation;
            return message;
        }

        private string GetInvoiceNumber(OrderConfirmationNotification notification, Svc.Core.Models.Profile.Customer customer)
        {
            string invoiceNumber = null;
            if (notification.InvoiceNumber != null)
            {
                invoiceNumber = notification.InvoiceNumber + " #" + notification.OrderNumber;
            }
            else
            {
                Order order = _orderHistoryLogic.GetOrder(customer.CustomerBranch, notification.OrderNumber);
                invoiceNumber = order.InvoiceNumber + " #" + notification.OrderNumber;
            }
            return invoiceNumber;
        }


        private decimal BuildOrderTable(OrderConfirmationNotification notification, Svc.Core.Models.Profile.Customer customer, StringBuilder originalOrderInfo)
        {
            StringBuilder itemOrderInfo = new StringBuilder();
            StringBuilder itemOrderInfoOOS = new StringBuilder();
            decimal totalAmount = 0;
            string catalog = null;
            catalog = notification.OrderChange.Items.Select(i => i.ItemCatalog).FirstOrDefault();
            if (catalog == null) catalog = customer.CustomerBranch;
            ProductsReturn products = _catRepo.GetProductsByIds(catalog, notification.OrderChange.Items.Select(i => i.ItemNumber.Trim()).ToList());
            foreach (var line in notification.OrderChange.Items)
            {
                Product currentProduct = products.Products.Where(i => i.ItemNumber == line.ItemNumber).FirstOrDefault();
                string priceInfo = BuildPriceInfo(line, currentProduct);
                if (!String.IsNullOrEmpty(line.OriginalStatus) && line.OriginalStatus.Equals("filled", StringComparison.CurrentCultureIgnoreCase))
                {
                    totalAmount += (line.QuantityOrdered * line.ItemPrice);
                    BuildItemDetail(itemOrderInfo, line, priceInfo, currentProduct);
                }
                else
                {
                    totalAmount += (line.QuantityShipped * line.ItemPrice);
                    BuildExceptionItemDetail(itemOrderInfoOOS, line, priceInfo, currentProduct);
                }
            }
            originalOrderInfo.Append(itemOrderInfoOOS);
            originalOrderInfo.Append(itemOrderInfo);

            return totalAmount;
        }

        private void BuildExceptionItemDetail(StringBuilder itemOrderInfoOOS, OrderLineChange line, string priceInfo, Product currentProduct)
        {
            if ((line != null) && (currentProduct != null) && (line.ItemNumber != null) && (currentProduct.Name != null) && (line.QuantityOrdered != null) && (line.QuantityShipped != null) && 
                (line.OriginalStatus != null) && (priceInfo != null))
            {
                MessageTemplateModel itemOOSDetailTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERITEMOOSDETAIL);
                itemOrderInfoOOS.Append(itemOOSDetailTemplate.Body.Inject(new
                {
                    ProductNumber = line.ItemNumber,
                    ProductDescription = currentProduct.Name,
                    Quantity = line.QuantityOrdered.ToString(),
                    Sent = line.QuantityShipped.ToString(),
                    Price = priceInfo,
                    Status = line.OriginalStatus
                }));
            }
        }

        private void BuildItemDetail(StringBuilder itemOrderInfo, OrderLineChange line, string priceInfo, Product currentProduct)
        {
            MessageTemplateModel itemDetailTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERITEMDETAIL);
            itemOrderInfo.Append(itemDetailTemplate.Body.Inject(new
            {
                ProductNumber = line.ItemNumber,
                ProductDescription = currentProduct.Name,
                Quantity = line.QuantityOrdered.ToString(),
                Sent = line.QuantityShipped.ToString(),
                Price = priceInfo,
                Status = line.OriginalStatus
            }));
        }

        private string BuildPriceInfo(OrderLineChange line, Product currentProduct)
        {
            StringBuilder priceInfo = new StringBuilder();
            if(line.ItemPrice != null) priceInfo.Append(line.ItemPrice.ToString("f2"));
            else priceInfo.Append("?");
            if ((currentProduct != null) && (currentProduct.CatchWeight))
            {
                priceInfo.Append(" lb per");
                if (line.Each) priceInfo.Append(" package");
                else priceInfo.Append(" case");
            }
            else
            {
                if (line.Each) priceInfo.Append(" per package");
                else priceInfo.Append(" per case");
            }
            return priceInfo.ToString();
        }

        private Message MakeRejectedMessage(OrderConfirmationNotification notification, Svc.Core.Models.Profile.Customer customer)
        {
            string invoiceNumber = GetInvoiceNumber(notification, customer);
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERREJECTED);
            Message message = new Message();

            message.MessageSubject = template.Subject.Inject(new
            {
                OrderStatus = "Order Rejected",
                CustomerNumber = customer.CustomerNumber,
                CustomerName = customer.CustomerName,
                InvoiceNumber = invoiceNumber
            });

            StringBuilder rejectedString = new StringBuilder();
            rejectedString.Append(template.Body.Inject(new
            {
                SpecialInstructions = notification.OrderChange.SpecialInstructions
            }));
            message.BodyIsHtml = template.IsBodyHtml;
            message.CustomerNumber = customer.CustomerNumber;
            message.CustomerName = customer.CustomerName;
            message.BranchId = customer.CustomerBranch;
            message.NotificationType = NotificationType.OrderConfirmation;
            return message;
        }
        #endregion
    }
}
