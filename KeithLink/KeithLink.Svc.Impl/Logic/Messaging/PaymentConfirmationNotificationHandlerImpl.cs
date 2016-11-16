using KeithLink.Common.Core.Extensions;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core;
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
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(Constants.MESSAGE_TEMPLATE_PAYMENTCONFIRMATION);
            MessageTemplateModel detailTemplate = _messageTemplateLogic.ReadForKey(Constants.MESSAGE_TEMPLATE_PAYMENTDETAIL);

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

        private Message GetEmailMessageForMultipleAccountSummaryNotification
            (List<PaymentTransactionModel> payments, List<UserSelectedContext> customers)
        {
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTCONFIRMATION);
            MessageTemplateModel headerTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTHEADER);
            MessageTemplateModel detailTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTDETAIL1);
            MessageTemplateModel detail2Template = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTDETAIL2);
            MessageTemplateModel detail3Template = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTDETAIL3);
            MessageTemplateModel footerAccountTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTERACCOUNT);
            MessageTemplateModel footerCustomerTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTERCUSTOMER);
            MessageTemplateModel footerGrandTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTERGRAND);
            MessageTemplateModel footerEndTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTEREND);

            StringBuilder orderDetails = new StringBuilder();

            int confirmationId = 0;
            string payer = null;
            decimal grandSum = 0;
            DateTime submittedDate = DateTime.MinValue;
            int customerNumber = 0;

            foreach (var customer in customers.OrderBy(ctx => ctx.CustomerId))
            { // the start of each customer
                ++customerNumber;

                Svc.Core.Models.Profile.Customer cust =
                    _customerRepo.GetCustomerByCustomerNumber(customer.CustomerId, customer.BranchId);

                int paymentNumber = 0;
                decimal paymentSum = 0;
                Core.Models.OnlinePayments.Customer.EF.CustomerBank bankUsed = null;

                foreach (var payment in payments.Where(p => p.CustomerNumber == customer.CustomerId &&
                                                            p.BranchId == customer.BranchId)
                                                .OrderBy(p => p.AccountNumber))
                {
                    paymentNumber++;

                    Core.Models.OnlinePayments.Customer.EF.CustomerBank bank = _bankRepo.GetBankAccount
                        (DivisionHelper.GetDivisionFromBranchId(customer.BranchId),
                        customer.CustomerId, payment.AccountNumber);

                    if(bankUsed == null || bankUsed.AccountNumber.Equals(bank.AccountNumber) == false)
                    {
                        if(bankUsed != null)
                        { // not sure if this happens, but wanted to provide for it just in case
                          // if bankused is not null but the bank account used changes, then we close out the table
                          // for the previous account
                            orderDetails.Append(footerAccountTemplate.Body.Inject(new
                            {
                                BankName = bankUsed.Name,
                                AccountNumber = bankUsed.AccountNumber,
                                AccountSum = paymentSum
                            }));

                            orderDetails.Append(footerEndTemplate.Body);
                        }

                        bankUsed = bank;

                        // this starts a table for the new account
                        orderDetails.Append(headerTemplate.Body.Inject(new
                        {
                            BankName = bankUsed.Name,
                            AccountNumber = bankUsed.AccountNumber
                        }));
                    }

                    paymentSum = paymentSum + payment.PaymentAmount;
                    grandSum = grandSum + payment.PaymentAmount;

                    Core.Models.OnlinePayments.Invoice.EF.Invoice invoice = 
                        _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId
                        (customer.BranchId), customer.CustomerId, payment.InvoiceNumber);
                    Core.Enumerations.InvoiceType invoiceTyped = 
                        KeithLink.Svc.Core.Extensions.InvoiceExtensions.DetermineType(invoice.InvoiceType);

                    confirmationId = payment.ConfirmationId;
                    payer = payment.UserName;
                    submittedDate = payment.PaymentDate.Value;

                    if (paymentNumber == 1)
                    {  // the following entries add details for the tables, the first line includes customer information
                        orderDetails.Append(detailTemplate.Body.Inject(new
                        {
                            CustomerNumber = cust.CustomerNumber,
                            CustomerBranch = cust.CustomerBranch,
                            CustomerName = cust.CustomerName,
                            InvoiceType = invoiceTyped,
                            InvoiceNumber = payment.InvoiceNumber,
                            InvoiceDate = invoice.InvoiceDate,
                            DueDate = invoice.DueDate,
                            PaymentAmount = payment.PaymentAmount
                        }));
                    }
                    else
                    {// the other data line alternate background color
                        if (paymentNumber % 2 == 1)
                        {
                            orderDetails.Append(detail2Template.Body.Inject(new
                            {
                                InvoiceType = invoiceTyped,
                                InvoiceNumber = payment.InvoiceNumber,
                                InvoiceDate = invoice.InvoiceDate,
                                DueDate = invoice.DueDate,
                                PaymentAmount = payment.PaymentAmount
                            }));
                        }
                        else
                        {
                            orderDetails.Append(detail3Template.Body.Inject(new
                            {
                                InvoiceType = invoiceTyped,
                                InvoiceNumber = payment.InvoiceNumber,
                                InvoiceDate = invoice.InvoiceDate,
                                DueDate = invoice.DueDate,
                                PaymentAmount = payment.PaymentAmount
                            }));
                        }
                    }
                }
                // the following appends a summation of the account used
                orderDetails.Append(footerAccountTemplate.Body.Inject(new
                {
                    BankName = bankUsed.Name,
                    AccountNumber = bankUsed.AccountNumber,
                    AccountSum = paymentSum
                }));
                // the following appends a summation of payments on the customer
                orderDetails.Append(footerCustomerTemplate.Body.Inject(new
                {
                    CustomerNumber = cust.CustomerNumber,
                    CustomerBranch = cust.CustomerBranch,
                    CustomerName = cust.CustomerName,
                    CustomerSum = paymentSum
                }));
                orderDetails.Append(footerEndTemplate.Body);
            }
            // the following appends a summation for all payments
            orderDetails.Append(footerGrandTemplate.Body.Inject(new
            {
                GrandSum = grandSum,
                ScheduledDate = submittedDate
            }));

            Message message = new Message();
            message.BodyIsHtml = template.IsBodyHtml;
            message.MessageSubject = template.Subject;
            // the following assembles the template for the body of the payer summary
            message.MessageBody = template.Body.Inject(new
            {
                UserName = payer,
                ConfirmationId = confirmationId,
                PaymentCollection = orderDetails.ToString()
            });
            message.NotificationType = NotificationType.PaymentConfirmation;
            return message;
        }

        public void ProcessNotification(BaseNotification notification)
        {
            try {
                if (notification.NotificationType != NotificationType.PaymentConfirmation)
                    throw new ApplicationException("notification/handler type mismatch");

                // had to setup a translation for this type in Svc.Core.Extensions to deserialize the message with the concrete type
                PaymentConfirmationNotification confirmation = (PaymentConfirmationNotification)notification;

                // UserContextEqualityComparer discriminates usercontext's to allow distinct to weed out duplicates
                List<UserSelectedContext> customerCtxs = confirmation.Payments
                                                           .Select(p => new UserSelectedContext() { CustomerId = p.CustomerNumber, BranchId = p.BranchId })
                                                           .Distinct(new UserContextEqualityComparer())
                                                           .ToList();

                bool complexPayment = customerCtxs.Count > 1;
                string payerEmail = confirmation.Payments.Select(p => p.UserName).FirstOrDefault();
                Recipient payerRecipient = null;

                foreach (UserSelectedContext customerCtx in customerCtxs) {
                    // load up recipients, customer and message
                    Svc.Core.Models.Profile.Customer customer = _customerRepo.GetCustomerByCustomerNumber(customerCtx.CustomerId, customerCtx.BranchId);

                    if (customer == null) {
                        StringBuilder warningMessage = new StringBuilder();
                        warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Payment Confirmation notification.", customerCtx.BranchId, customerCtx.CustomerId);
                        warningMessage.AppendLine();
                        warningMessage.AppendLine();
                        warningMessage.AppendLine("Notification:");
                        warningMessage.AppendLine(notification.ToJson());

                        _log.WriteWarningLog(warningMessage.ToString());
                    }
                    else {
                        List<Recipient> recipients = LoadRecipients(confirmation.NotificationType, customer);
                        Message message = GetEmailMessageForNotification(confirmation.Payments
                                                                                     .Where(p => p.CustomerNumber == customerCtx.CustomerId && p.BranchId == customerCtx.BranchId)
                                                                                     .ToList(),
                                                                         customer);

                        // send messages to providers...
                        if (recipients != null && recipients.Count > 0) {
                            if (complexPayment) // mask out payeremail recipient from the regular recipients
                            {
                                payerRecipient = recipients.Where(r => r.UserEmail == payerEmail).FirstOrDefault();
                                recipients = recipients.Where(r => r.UserEmail != payerEmail).ToList();
                            }

                            if(recipients.Count > 0)
                            {
                                SendMessage(recipients, message);
                            }
                        }
                    }
                }

                if (complexPayment && payerRecipient != null)
                {
                    List<Recipient> recips = new List<Recipient>();
                    recips.Add(payerRecipient);
                    SendMessage(recips,
                        GetEmailMessageForMultipleAccountSummaryNotification(confirmation.Payments, customerCtxs));
                }
            }
            catch (Exception ex) {
                throw new Core.Exceptions.Queue.QueueDataError<BaseNotification>(notification, "PaymentConfirmation:ProcessNotification", "Sending PaymentConfirmation notification", "An error occured processing a payment confirmation", ex);
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