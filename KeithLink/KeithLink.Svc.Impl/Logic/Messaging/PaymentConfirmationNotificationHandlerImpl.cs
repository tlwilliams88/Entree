using KeithLink.Common.Core.Extensions;

using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;

using KeithLink.Svc.Core.Extensions.Messaging;

using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Messaging.EF;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.OnlinePayments.Customer;
using KeithLink.Svc.Core.Models.OnlinePayments.Payment;
using KeithLink.Svc.Core.Models.SiteCatalog;
//using KeithLink.Svc.Core.Models.Profile;

using KeithLink.Svc.Impl.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class PaymentConfirmationNotificationHandlerImpl : BaseNotificationHandlerImpl, INotificationHandler
    {
        #region attributes
        private const string MESSAGE_TEMPLATE_PAYMENTCONFIRMATION = "PaymentConfirmation";
        private const string MESSAGE_TEMPLATE_PAYMENTDETAIL = "PaymentConfirmationDetail";

        private readonly IEventLogRepository _log;
        private readonly IUserProfileLogic _userLogic;
        private readonly IUserPushNotificationDeviceRepository _pushRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IUserMessagingPreferenceRepository _userPreferenceRepo;
        private readonly IMessageTemplateLogic _messageTemplateLogic;
        private readonly IKPayInvoiceRepository _invoiceRepo;
        private readonly ICustomerBankRepository _bankRepo;
        #endregion

        #region ctor
        public PaymentConfirmationNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic,
                                                          IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository,
                                                          IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory,
                                                          IMessageTemplateLogic messageTemplateLogic, IKPayInvoiceRepository kpayInvoiceRepo, ICustomerBankRepository customerBankRepo,
                                                          IDsrLogic dsrLogic)
            : base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository,
                   userMessagingPreferenceRepository, messageProviderFactory, eventLogRepository, dsrLogic)
        {
            _log = eventLogRepository;
            _userLogic = userProfileLogic;
            _pushRepo = userPushNotificationDeviceRepository;
            _customerRepo = customerRepository;
            _userPreferenceRepo = userMessagingPreferenceRepository;
            _messageTemplateLogic = messageTemplateLogic;
            _invoiceRepo = kpayInvoiceRepo;
            _bankRepo = customerBankRepo;
        }
        #endregion

        #region methods
        private Message GetEmailMessageForNotification(List<PaymentTransactionModel> payments, Core.Models.Profile.Customer customer)
        {
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_PAYMENTCONFIRMATION);
            MessageTemplateModel detailTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_PAYMENTDETAIL);

            StringBuilder orderDetails = new StringBuilder();

            foreach (var payment in payments)
            {
                var invoice = _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(customer.CustomerBranch), customer.CustomerNumber, payment.InvoiceNumber);
                var invoiceTyped = KeithLink.Svc.Core.Extensions.InvoiceExtensions.DetermineType(invoice.InvoiceType);
                orderDetails.Append(detailTemplate.Body.Inject(new
                {
                    InvoiceType = invoiceTyped,
                    InvoiceNumber = payment.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    DueDate = invoice.DueDate,
                    ScheduledDate = payment.PaymentDate,
                    PaymentAmount = payment.PaymentAmount
                }));
            }


            var bank = _bankRepo.GetBankAccount(DivisionHelper.GetDivisionFromBranchId(customer.CustomerBranch), customer.CustomerNumber, payments[0].AccountNumber);
            var confirmationId = payments[0].ConfirmationId;

            Message message = new Message();
            message.BodyIsHtml = template.IsBodyHtml;
            message.MessageSubject = template.Subject.Inject(customer);
            StringBuilder header = _messageTemplateLogic.BuildHeader("Thank you for your payment", customer);
            message.MessageBody = template.Body.Inject(new
            {
                NotifHeader = header.ToString(),
                ConfirmationId = confirmationId,
                BankAccount = bank.AccountNumber + " - " + bank.Name,
                PaymentDetailLines = orderDetails.ToString(),
                TotalPayments = payments.Sum(p => p.PaymentAmount)
            });
            message.CustomerNumber = customer.CustomerNumber;
            message.CustomerName = customer.CustomerName;
            message.BranchId = customer.CustomerBranch;
            message.NotificationType = NotificationType.PaymentConfirmation;
            return message;
        }

        public void ProcessNotification(BaseNotification notification)
        {
            if (notification.NotificationType != NotificationType.PaymentConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            // had to setup a translation for this type in Svc.Core.Extensions to deserialize the message with the concrete type
            PaymentConfirmationNotification confirmation = (PaymentConfirmationNotification)notification;

            // UserContextEqualityComparer discriminates usercontext's to allow distinct to weed out duplicates
            List<UserSelectedContext> customerCtxs = confirmation.Payments
                                                       .Select(p => new UserSelectedContext() { CustomerId = p.CustomerNumber, BranchId = p.BranchId })
                                                       .Distinct(new UserContextEqualityComparer())
                                                       .ToList();

            foreach (UserSelectedContext customerCtx in customerCtxs)
            {
                // load up recipients, customer and message
                Svc.Core.Models.Profile.Customer customer = _customerRepo.GetCustomerByCustomerNumber(customerCtx.CustomerId, customerCtx.BranchId);

                if (customer == null)
                {
                    StringBuilder warningMessage = new StringBuilder();
                    warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Payment Confirmation notification.", customerCtx.BranchId, customerCtx.CustomerId);
                    warningMessage.AppendLine();
                    warningMessage.AppendLine();
                    warningMessage.AppendLine("Notification:");
                    warningMessage.AppendLine(notification.ToJson());

                    _log.WriteWarningLog(warningMessage.ToString());
                }
                else
                {
                    List<Recipient> recipients = LoadRecipients(confirmation.NotificationType, customer);
                    Message message = GetEmailMessageForNotification(confirmation.Payments
                                                                                 .Where(p => p.CustomerNumber == customerCtx.CustomerId && p.BranchId == customerCtx.BranchId)
                                                                                 .ToList(), 
                                                                     customer);

                    // send messages to providers...
                    if (recipients != null && recipients.Count > 0)
                    {
                        SendMessage(recipients, message);
                    }
                }
            }
        }

        //public void ProcessNotificationForExternalUsers(BaseNotification notification)
        //{
        //    if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.PaymentConfirmation)
        //        throw new ApplicationException("notification/handler type mismatch");

        //    // had to setup a translation for this type in Svc.Core.Extensions to deserialize the message with the concrete type
        //    PaymentConfirmationNotification confirmation = (PaymentConfirmationNotification)notification;

        //    // load up recipients, customer and message
        //    Svc.Core.Models.Profile.Customer customer = _customerRepo.GetCustomerByCustomerNumber(confirmation.CustomerNumber, confirmation.BranchId);

        //    if (customer == null)
        //    {
        //        System.Text.StringBuilder warningMessage = new StringBuilder();
        //        warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Payment Confirmation notification.", notification.BranchId, notification.CustomerNumber);
        //        warningMessage.AppendLine();
        //        warningMessage.AppendLine();
        //        warningMessage.AppendLine("Notification:");
        //        warningMessage.AppendLine(notification.ToJson());

        //        _log.WriteWarningLog(warningMessage.ToString());
        //    }
        //    else
        //    {
        //        List<Recipient> recipients = base.LoadRecipients(confirmation.NotificationType, customer, false, false, true);
        //        Message message = GetEmailMessageForNotification(confirmation, customer);

        //        // send messages to providers...
        //        if (recipients != null && recipients.Count > 0)
        //        {
        //            try
        //            {
        //                base.SendMessage(recipients, message);
        //            }
        //            catch (Exception ex)
        //            {
        //                _log.WriteErrorLog(String.Format("Error sending messages {0} {1}", ex.Message, ex.StackTrace));
        //            }
        //        }
        //    }
        //}

        //public void ProcessNotificationForInternalUsers(BaseNotification notification)
        //{
        //    if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.PaymentConfirmation)
        //        throw new ApplicationException("notification/handler type mismatch");

        //    // had to setup a translation for this type in Svc.Core.Extensions to deserialize the message with the concrete type
        //    PaymentConfirmationNotification confirmation = (PaymentConfirmationNotification)notification;

        //    // load up recipients, customer and message
        //    Svc.Core.Models.Profile.Customer customer = _customerRepo.GetCustomerByCustomerNumber(confirmation.CustomerNumber, confirmation.BranchId);

        //    if (customer == null)
        //    {
        //        System.Text.StringBuilder warningMessage = new StringBuilder();
        //        warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Payment Confirmation notification.", notification.BranchId, notification.CustomerNumber);
        //        warningMessage.AppendLine();
        //        warningMessage.AppendLine();
        //        warningMessage.AppendLine("Notification:");
        //        warningMessage.AppendLine(notification.ToJson());

        //        _log.WriteWarningLog(warningMessage.ToString());
        //    }
        //    else
        //    {
        //        List<Recipient> recipients = base.LoadRecipients(confirmation.NotificationType, customer, false, true, false);
        //        Message message = GetEmailMessageForNotification(confirmation, customer);

        //        // send messages to providers...
        //        if (recipients != null && recipients.Count > 0)
        //        {
        //            try
        //            {
        //                base.SendMessage(recipients, message);
        //            }
        //            catch (Exception ex)
        //            {
        //                _log.WriteErrorLog(String.Format("Error sending messages {0} {1}", ex.Message, ex.StackTrace));
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}