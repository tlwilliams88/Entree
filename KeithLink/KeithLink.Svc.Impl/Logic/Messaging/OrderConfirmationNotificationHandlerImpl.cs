using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;

using KeithLink.Svc.Core.Extensions.Messaging;

using KeithLink.Svc.Core.Helpers;

using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private readonly IPriceLogic _priceLogic;
        IEventLogRepository eventLogRepository;
        IUserProfileLogic userProfileLogic;
        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository;
        ICustomerRepository customerRepository;
        private readonly IMessageTemplateLogic _messageTemplateLogic;
        IUserMessagingPreferenceRepository userMessagingPreferenceRepository;
        Func<Channel, IMessageProvider> messageProviderFactory;
        private readonly IDsrLogic dsrLogic;
        private IOrderLogic _orderLogic;
        #endregion

        #region ctor
        public OrderConfirmationNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic,
                                                        IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository,
                                                        IMessageTemplateLogic messageTemplateLogic, ICustomerRepository customerRepository,
                                                        IUserMessagingPreferenceRepository userMessagingPreferenceRepository,
                                                        Func<Channel, IMessageProvider> messageProviderFactory,
                                                        IDsrLogic dsrLogic, ICatalogRepository catalogRepository,
                                                        IOrderLogic orderLogic, IPriceLogic priceLogic)
            : base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository,
                     userMessagingPreferenceRepository, messageProviderFactory, eventLogRepository,
                     dsrLogic)
        {
            _priceLogic = priceLogic;
            _catRepo = catalogRepository;
            this.eventLogRepository = eventLogRepository;
            this.userProfileLogic = userProfileLogic;
            this.userPushNotificationDeviceRepository = userPushNotificationDeviceRepository;
            this.customerRepository = customerRepository;
            this.userMessagingPreferenceRepository = userMessagingPreferenceRepository;
            _messageTemplateLogic = messageTemplateLogic;
            this.messageProviderFactory = messageProviderFactory;
            this._orderLogic = orderLogic;
        }
        #endregion

        #region methods
        private void BuildExceptionItemDetail(StringBuilder itemOrderInfoOOS, OrderLineChange line, string priceInfo, string extPriceInfo, Product currentProduct) {
            MessageTemplateModel itemOOSDetailTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERITEMOOSDETAIL);
            StringBuilder number = new StringBuilder();
            number.Append(line.ItemNumber);
            if (line.SubstitutedItemNumber != null && line.SubstitutedItemNumber.Trim().Length > 0)
            {
                number.Append(" to " + line.SubstitutedItemNumber);
            }
            StringBuilder status = new StringBuilder();
            status.Append(line.OriginalStatus);
            if (line.NewStatus != null && line.NewStatus.Trim().Length > 0)
            {
                status.Append(" to " + line.NewStatus);
            }

            itemOrderInfoOOS.Append(itemOOSDetailTemplate.Body.Inject(new {
                ProductNumber = number.ToString(),
                ProductDescription = currentProduct.Name,
                Brand = currentProduct.Brand,
                Quantity = line.QuantityOrdered.ToString(),
                Sent = line.QuantityShipped.ToString(),
                Pack = currentProduct.Pack,
                Size = currentProduct.Size,
                Price = priceInfo,
                Status = status.ToString()
            }));
        }

        private string BuildExtPriceInfo(OrderLineChange line, Product currentProduct) {
            string priceExtInfo = currentProduct.CasePrice;
            priceExtInfo += " per case";
            if(currentProduct.CaseOnly == false) {
                priceExtInfo += "/";
                priceExtInfo += currentProduct.PackagePrice;
                priceExtInfo += " per package";
            }
            return priceExtInfo;
        }

        private void BuildItemDetail(StringBuilder itemOrderInfo, OrderLineChange line, string priceInfo, string extPriceInfo, Product currentProduct) {
            MessageTemplateModel itemDetailTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERITEMDETAIL);
            itemOrderInfo.Append(itemDetailTemplate.Body.Inject(new {
                ProductNumber = line.ItemNumber,
                ProductDescription = currentProduct.Name,
                Brand = currentProduct.Brand,
                Quantity = line.QuantityOrdered.ToString(),
                Sent = line.QuantityOrdered.ToString(),
                Pack = currentProduct.Pack,
                Size = currentProduct.Size,
                Price = priceInfo,
                Status = line.OriginalStatus
            }));
        }

        private void BuildNotificationChanges(OrderConfirmationNotification notification, StringBuilder orderLineChanges) {
            MessageTemplateModel changeDetailTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERCHANGEDETAIL);
            foreach(var line in notification.OrderChange.ItemChanges) {
                //orderLineChanges += orderLineChanges + "Item: " + line.ItemNumber +
                //    (String.IsNullOrEmpty(line.SubstitutedItemNumber) ? string.Empty : ("replace by: " + line.SubstitutedItemNumber)) +
                //    "  Status: " + line.NewStatus + (line.NewStatus == line.OriginalStatus || string.IsNullOrEmpty(line.OriginalStatus)
                //                                        ? string.Empty : (" change from: " + line.OriginalStatus)) + System.Environment.NewLine;
                StringBuilder number = new StringBuilder();
                number.Append(line.ItemNumber);
                if(line.SubstitutedItemNumber != null && line.SubstitutedItemNumber.Trim().Length>0)
                {
                    number.Append(" to " + line.SubstitutedItemNumber);
                }
                StringBuilder status = new StringBuilder();
                status.Append(line.OriginalStatus);
                if (line.NewStatus != null && line.NewStatus.Trim().Length > 0)
                {
                    status.Append(" to " + line.NewStatus);
                }
                orderLineChanges.Append(changeDetailTemplate.Body.Inject(new {
                    ProductNumber = number.ToString(),
                    Status = status.ToString()
                }));

            }
        }

        private decimal BuildOrderTables(OrderConfirmationNotification notification, Customer customer, StringBuilder orderLineChanges, StringBuilder originalOrderInfo) {
            StringBuilder itemOrderInfo = new StringBuilder();
            StringBuilder itemOrderInfoOOS = new StringBuilder();
            MessageTemplateModel itemsTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERITEMS);
            MessageTemplateModel itemsOOSTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERITEMSOOS);
            decimal totalAmount = 0;
            ProductsReturn products = _catRepo.GetProductsByIds(customer.CustomerBranch, notification.OrderChange.Items.Select(i => i.ItemNumber).ToList());
            var pricing = _priceLogic.GetPrices(customer.CustomerBranch, customer.CustomerNumber, DateTime.Now.AddDays(1), products.Products);
            foreach(var line in notification.OrderChange.Items) {
                Product currentProduct = products.Products.Where(i => i.ItemNumber == line.ItemNumber).FirstOrDefault();
                if(currentProduct == null)
                {
                    currentProduct = _catRepo.GetProductById(line.ItemCatalog, line.ItemNumber);
                }
                var price = pricing.Prices.Where(p => p.ItemNumber.Equals(line.ItemNumber)).FirstOrDefault();
                string priceInfo = BuildPriceInfo(line, currentProduct);
                string extPriceInfo = BuildExtPriceInfo(line, currentProduct);
                if(line.OriginalStatus == null) {
                    return -1;
                }
                if(line.OriginalStatus.Equals("filled", StringComparison.CurrentCultureIgnoreCase)) {
                    totalAmount += GetLinePrice(line, currentProduct, price);
                    BuildItemDetail(itemOrderInfo, line, priceInfo, extPriceInfo, currentProduct);
                } else {
                    totalAmount += GetLinePrice(line, currentProduct, price);
                    BuildExceptionItemDetail(itemOrderInfoOOS, line, priceInfo, extPriceInfo, currentProduct);
                }
            }
            if(itemOrderInfoOOS.Length > 0) {
                originalOrderInfo.Append(itemsOOSTemplate.Body.Inject(new {
                    OrderConfirmationItemOOSDetail = itemOrderInfoOOS.ToString()
                }));
            }
            if(orderLineChanges.Length > 0) {
                MessageTemplateModel changeTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERCHANGE);
                originalOrderInfo.Append(changeTemplate.Body.Inject(new {
                    OrderChangeLines = orderLineChanges.ToString()
                }));
            }
            if(itemOrderInfo.Length > 0) {
                originalOrderInfo.Append(itemsTemplate.Body.Inject(new {
                    OrderConfirmationItemDetail = itemOrderInfo.ToString()
                }));
            }
            return totalAmount;
        }

        private static int BuildPieceCount(OrderConfirmationNotification notification)
        {
            int pieces = 0;
            foreach (OrderLineChange item in notification.OrderChange.Items)
            {
                pieces += item.QuantityShipped;
            }
            return pieces;
        }

        private string BuildPriceInfo(OrderLineChange line, Product currentProduct) {
            string priceInfo = line.ItemPrice.ToString("f2");
            if (currentProduct.CatchWeight)
            {
                priceInfo += " per lb";
            }
            else if (line.Each)
            {
                priceInfo += " per package";
            }
            else
            {
                priceInfo += " per case";
            }
            return priceInfo;
        }

        private Message GetEmailMessageForNotification(OrderConfirmationNotification notification, Customer customer) {
            StringBuilder statusString = new StringBuilder();
            if(notification.OrderChange.CurrentStatus.Equals("rejected", StringComparison.CurrentCultureIgnoreCase)) {
                return MakeRejectedMessage(notification, customer);
            }
            StringBuilder orderLineChanges = new StringBuilder();
            if((notification.OrderChange.ItemChanges != null) && (notification.OrderChange.ItemChanges.Count > 0)) {
                BuildNotificationChanges(notification, orderLineChanges);
            }
            StringBuilder originalOrderInfo = new StringBuilder();
            decimal totalAmount = BuildOrderTables(notification, customer, orderLineChanges, originalOrderInfo);
                return MakeConfirmationMessage(notification, customer, originalOrderInfo, totalAmount);
        }

        private string GetInvoiceNumber(OrderConfirmationNotification notification, Customer customer) {
            string invoiceNumber = null;
            if(notification.InvoiceNumber != null) {
                invoiceNumber = notification.InvoiceNumber;
            } else {
                UserSelectedContext usc = new UserSelectedContext();
                usc.CustomerId = customer.CustomerNumber;
                usc.BranchId = customer.CustomerBranch;
                DateTime today = DateTime.Parse(DateTime.Now.ToShortDateString());
                List<string> notifItems = notification.OrderChange.Items.Select(x => x.ItemNumber).ToList();
                List<Order> orders = _orderLogic.GetOrderHeaderInDateRange(usc, today, today.AddDays(1))
                                        .ToList();
                if((orders != null) && (orders.Count > 0))
                    invoiceNumber = orders[0].InvoiceNumber;
            }
            return invoiceNumber;
        }

        private static decimal GetLinePrice(OrderLineChange line, Product currentProduct, Price price) {
            if(currentProduct.CatchWeight) {
                if(line.Each) //package catchweight
                {
                    return (decimal)PricingHelper.GetCatchweightPriceForPackage
                        (line.QuantityShipped, int.Parse(currentProduct.Pack), currentProduct.AverageWeight, price.PackagePrice);
                } else //case catchweight
                  {
                    return (decimal)PricingHelper.GetCatchweightPriceForCase
                        (line.QuantityShipped, currentProduct.AverageWeight, price.CasePrice);
                }
            } else {
                return line.QuantityShipped * line.ItemPrice;
            }
        }

        private Message MakeConfirmationMessage(OrderConfirmationNotification notification, Customer customer, StringBuilder originalOrderInfo, decimal totalAmount) {
            string invoiceNumber = GetInvoiceNumber(notification, customer);
            Message message = new Message();
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERCONFIRMATION);
            message.MessageSubject = template.Subject.Inject(new {
                OrderStatus = "Order Confirmation",
                CustomerNumber = customer.CustomerNumber,
                CustomerName = customer.CustomerName,
                InvoiceNumber = invoiceNumber
            });
            StringBuilder header = _messageTemplateLogic.BuildHeader("Thank you for your order", customer);
            message.MessageBody += template.Body.Inject(new {
                NotifHeader = header.ToString(),
                InvoiceNumber = invoiceNumber,
                ShipDate = notification.OrderChange.ShipDate,
                Count = notification.OrderChange.Items.Count,
                PcsCount = BuildPieceCount(notification),
                Total = totalAmount.ToString("f2"),
                PurchaseOrder = notification.OrderChange.OrderName,
                OrderConfirmationItems = originalOrderInfo.ToString()
            });
            message.BodyIsHtml = template.IsBodyHtml;
            message.CustomerNumber = customer.CustomerNumber;
            message.CustomerName = customer.CustomerName;
            message.BranchId = customer.CustomerBranch;
            message.NotificationType = NotificationType.OrderConfirmation;

            //AlternateView avHtml = AlternateView.CreateAlternateViewFromString
            //    (message.MessageBody, null, MediaTypeNames.Text.Html);

            //// Create a LinkedResource object for each embedded image
            //LinkedResource pic1 = new LinkedResource("pic.jpg", MediaTypeNames.Image.Jpeg);
            //pic1.ContentId = "Pic1";
            //avHtml.LinkedResources.Add(pic1);

            return message;
        }

        private Message MakeRejectedMessage(OrderConfirmationNotification notification, Customer customer) {
            MessageTemplateModel rejectTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_ORDERREJECTED);
            Message message = new Message();
            message.MessageSubject = rejectTemplate.Subject.Inject(new {
                CustomerNumber = customer.CustomerNumber,
                CustomerName = customer.CustomerName,
            });
            StringBuilder header = _messageTemplateLogic.BuildHeader("Order Rejected", customer);
            message.MessageBody = rejectTemplate.Body.Inject(new {
                NotifHeader = header.ToString(),
                SpecialInstructions = notification.OrderChange.SpecialInstructions
            });
            message.BodyIsHtml = rejectTemplate.IsBodyHtml;
            message.CustomerNumber = customer.CustomerNumber;
            message.CustomerName = customer.CustomerName;
            message.BranchId = customer.CustomerBranch;
            message.NotificationType = NotificationType.OrderConfirmation;
            return message;
        }

        public void ProcessNotification(BaseNotification notification)
        {
            if (notification.NotificationType != NotificationType.OrderConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            OrderConfirmationNotification orderConfirmation = (OrderConfirmationNotification)notification;

            // load up recipients, customer and message
            eventLogRepository.WriteInformationLog("order confirmation base, custNum: " + notification.CustomerNumber + ", branch: " + notification.BranchId);
            Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber, notification.BranchId);

            if (customer == null)
            {
                StringBuilder warningMessage = new StringBuilder();
                warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Order Confirmation notification.", notification.BranchId, notification.CustomerNumber);
                warningMessage.AppendLine();
                warningMessage.AppendLine();
                warningMessage.AppendLine("Notification:");
                warningMessage.AppendLine(notification.ToJson());

                eventLogRepository.WriteWarningLog(warningMessage.ToString());
            }
            else
            {
                List<Recipient> recipients = LoadRecipients(orderConfirmation.NotificationType, customer);
                Message message = GetEmailMessageForNotification(orderConfirmation, customer);

                // send messages to providers...
                if (recipients != null && recipients.Count > 0)
                {
                    SendMessage(recipients, message);
                }
            }
        }

        //public void ProcessNotificationForExternalUsers(Core.Models.Messaging.Queue.BaseNotification notification)
        //{
        //    if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.OrderConfirmation)
        //        throw new ApplicationException("notification/handler type mismatch");

        //    OrderConfirmationNotification orderConfirmation = (OrderConfirmationNotification)notification;

        //    // load up recipients, customer and message
        //    eventLogRepository.WriteInformationLog("order confirmation ext, custNum: " + notification.CustomerNumber + ", branch: " + notification.BranchId);
        //    Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber, notification.BranchId);

        //    if (customer == null)
        //    {
        //        System.Text.StringBuilder warningMessage = new StringBuilder();
        //        warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Order Confirmation notification.", notification.BranchId, notification.CustomerNumber);
        //        warningMessage.AppendLine();
        //        warningMessage.AppendLine();
        //        warningMessage.AppendLine("Notification:");
        //        warningMessage.AppendLine(notification.ToJson());

        //        eventLogRepository.WriteWarningLog(warningMessage.ToString());
        //    }
        //    else
        //    {
        //        List<Recipient> recipients = base.LoadRecipients(orderConfirmation.NotificationType, customer, false, false, true);

        //        // send messages to providers...
        //        if (recipients != null && recipients.Count > 0)
        //        {
        //            try
        //            {
        //                Message message = GetEmailMessageForNotification(orderConfirmation, customer);
        //                base.SendMessage(recipients, message);
        //            }
        //            catch (Exception ex)
        //            {
        //                eventLogRepository.WriteErrorLog(String.Format("Error sending messages {0} {1}", ex.Message, ex.StackTrace));
        //            }
        //        }
        //    }
        //}

        //public void ProcessNotificationForInternalUsers(Core.Models.Messaging.Queue.BaseNotification notification)
        //{
        //    if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.OrderConfirmation)
        //        throw new ApplicationException("notification/handler type mismatch");

        //    OrderConfirmationNotification orderConfirmation = (OrderConfirmationNotification)notification;

        //    // load up recipients, customer and message
        //    eventLogRepository.WriteInformationLog("order confirmation int, custNum: " + notification.CustomerNumber + ", branch: " + notification.BranchId);
        //    Svc.Core.Models.Profile.Customer customer = customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber, notification.BranchId);

        //    if (customer == null)
        //    {
        //        System.Text.StringBuilder warningMessage = new StringBuilder();
        //        warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Order Confirmation notification.", notification.BranchId, notification.CustomerNumber);
        //        warningMessage.AppendLine();
        //        warningMessage.AppendLine();
        //        warningMessage.AppendLine("Notification:");
        //        warningMessage.AppendLine(notification.ToJson());

        //        eventLogRepository.WriteWarningLog(warningMessage.ToString());
        //    }
        //    else
        //    {
        //        List<Recipient> recipients = base.LoadRecipients(orderConfirmation.NotificationType, customer, false, true, false);

        //        // send messages to providers...
        //        if (recipients != null && recipients.Count > 0)
        //        {
        //            try
        //            {
        //                eventLogRepository.WriteInformationLog("order confirmation int, custNum: " +
        //                                                       notification.CustomerNumber +
        //                                                       ", branch: " +
        //                                                       notification.BranchId +
        //                                                       ", recipients " +
        //                                                       recipients.Count);
        //                Message message = GetEmailMessageForNotification(orderConfirmation, customer);
        //                base.SendMessage(recipients, message);
        //            }
        //            catch (Exception ex)
        //            {
        //                eventLogRepository.WriteErrorLog(String.Format("Error sending messages {0} {1}", ex.Message, ex.StackTrace));
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
