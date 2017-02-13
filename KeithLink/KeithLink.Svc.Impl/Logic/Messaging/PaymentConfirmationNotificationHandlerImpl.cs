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

        public const string BANK_RESOLVE_UNDEFINED = "undefined";
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
                BankAccount = GetBankAccountNumber(bank) + " - " + GetBankName(bank),
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
            StringBuilder orderDetails = new StringBuilder();

            int confirmationId = 0, customerNumber = 0;
            string payer = null;
            decimal grandSum = 0;
            DateTime submittedDate = DateTime.MinValue;

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
                    bankUsed = BuildPaymentSummaryBankUsed(orderDetails, paymentSum, bankUsed, bank);

                    paymentSum = paymentSum + payment.PaymentAmount;
                    grandSum = grandSum + payment.PaymentAmount;

                    confirmationId = payment.ConfirmationId;
                    payer = payment.UserName;
                    submittedDate = payment.PaymentDate.Value;

                    BuildPaymentSummaryPaymentDetails(orderDetails, customer, cust, paymentNumber, payment);
                }
                BuildPaymentSummaryFooter(orderDetails, cust, paymentSum, bankUsed);
            }
            ApplyGrandPaymentSummaryTemplate(orderDetails, grandSum, submittedDate);

            return AssembleMessageForPayerSummary(orderDetails, confirmationId, payer);
        }

        private void BuildPaymentSummaryFooter(StringBuilder orderDetails, Core.Models.Profile.Customer cust, decimal paymentSum, Core.Models.OnlinePayments.Customer.EF.CustomerBank bankUsed)
        {
            ApplyAccountPaymentSummaryTemplate(orderDetails, bankUsed, paymentSum);

            ApplyCustomerPaymentSummaryTemplate(orderDetails, cust, paymentSum);

            ApplyEndFooterTemplate(orderDetails);
        }

        private Core.Models.OnlinePayments.Customer.EF.CustomerBank BuildPaymentSummaryBankUsed(StringBuilder orderDetails, decimal paymentSum, Core.Models.OnlinePayments.Customer.EF.CustomerBank bankUsed, Core.Models.OnlinePayments.Customer.EF.CustomerBank bank)
        {
            if (bankUsed == null || bankUsed.AccountNumber.Equals(bank.AccountNumber) == false)
            {
                InPaymentSummaryAccountForMultiplePaymentAccountsWithOneCustomer
                    (orderDetails, paymentSum, bankUsed);

                bankUsed = bank;

                ApplyPaymentSummaryAccountHeaderTemplate(orderDetails, bankUsed);
            }

            return bankUsed;
        }

        private void InPaymentSummaryAccountForMultiplePaymentAccountsWithOneCustomer
            (StringBuilder orderDetails, decimal paymentSum, Core.Models.OnlinePayments.Customer.EF.CustomerBank bankUsed)
        {
            if (bankUsed != null)
            { // not sure if this happens, but wanted to provide for it just in case
              // if bankused is not null but the bank account used changes, then we close out the table
              // for the previous account
                ApplyAccountPaymentSummaryTemplate(orderDetails, bankUsed, paymentSum);

                ApplyEndFooterTemplate(orderDetails);
            }
        }

        private void BuildPaymentSummaryPaymentDetails
            (StringBuilder orderDetails, UserSelectedContext customer, 
             Core.Models.Profile.Customer cust, int paymentNumber, PaymentTransactionModel payment)
        {
            Core.Models.OnlinePayments.Invoice.EF.Invoice invoice;
            Core.Enumerations.InvoiceType invoiceTyped;
            GetPaymentInvoiceInformation(customer, payment, out invoice, out invoiceTyped);

            if (paymentNumber == 1)
            {
                ApplyPaymentSummaryDetailsWithCustomerTemplate
                    (orderDetails, cust, payment, invoice, invoiceTyped);
            }
            else
            {
                if (paymentNumber % 2 == 1)
                {
                    ApplyPaymentSummaryDetailsTemplate(orderDetails, payment, invoice, invoiceTyped);
                }
                else
                {
                    ApplyPaymentSummaryDetailsAltTemplate(orderDetails, payment, invoice, invoiceTyped);
                }
            }
        }

        private void ApplyPaymentSummaryAccountHeaderTemplate(StringBuilder orderDetails, Core.Models.OnlinePayments.Customer.EF.CustomerBank bankUsed)
        {
            // this starts a table for the new account
            MessageTemplateModel headerTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTHEADER);
            orderDetails.Append(headerTemplate.Body.Inject(new
            {
                BankName = GetBankName(bankUsed),
                AccountNumber = GetBankAccountNumber(bankUsed)
            }));
        }

        private void GetPaymentInvoiceInformation(UserSelectedContext customer, PaymentTransactionModel payment, out Core.Models.OnlinePayments.Invoice.EF.Invoice invoice, out Core.Enumerations.InvoiceType invoiceTyped)
        {
            invoice = _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId
                (customer.BranchId), customer.CustomerId, payment.InvoiceNumber);
            invoiceTyped = KeithLink.Svc.Core.Extensions.InvoiceExtensions.DetermineType(invoice.InvoiceType);
        }

        private void ApplyPaymentSummaryDetailsAltTemplate(StringBuilder orderDetails, PaymentTransactionModel payment, Core.Models.OnlinePayments.Invoice.EF.Invoice invoice, Core.Enumerations.InvoiceType invoiceTyped)
        {
            MessageTemplateModel detail3Template = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTDETAIL3);
            orderDetails.Append(detail3Template.Body.Inject(new
            {
                InvoiceType = invoiceTyped,
                InvoiceNumber = payment.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                PaymentAmount = payment.PaymentAmount
            }));
        }

        private void ApplyPaymentSummaryDetailsTemplate(StringBuilder orderDetails, PaymentTransactionModel payment, Core.Models.OnlinePayments.Invoice.EF.Invoice invoice, Core.Enumerations.InvoiceType invoiceTyped)
        {
            MessageTemplateModel detail2Template = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTDETAIL2);
            orderDetails.Append(detail2Template.Body.Inject(new
            {
                InvoiceType = invoiceTyped,
                InvoiceNumber = payment.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                PaymentAmount = payment.PaymentAmount
            }));
        }

        private void ApplyPaymentSummaryDetailsWithCustomerTemplate(StringBuilder orderDetails, Core.Models.Profile.Customer cust, PaymentTransactionModel payment, Core.Models.OnlinePayments.Invoice.EF.Invoice invoice, Core.Enumerations.InvoiceType invoiceTyped)
        {
            MessageTemplateModel detailTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTDETAIL1);
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

        private Message AssembleMessageForPayerSummary
            (StringBuilder orderDetails, int confirmationId, string payer)
        {
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTCONFIRMATION);

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

        private void ApplyAccountPaymentSummaryTemplate
            (StringBuilder orderDetails, Core.Models.OnlinePayments.Customer.EF.CustomerBank bankUsed, decimal paymentSum)
        {
            // the following appends a summation of the account used
            MessageTemplateModel footerAccountTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTERACCOUNT);
            orderDetails.Append(footerAccountTemplate.Body.Inject(new
            {
                BankName = GetBankName(bankUsed),
                AccountNumber = GetBankAccountNumber(bankUsed),
                AccountSum = paymentSum
            }));
        }

        private void ApplyCustomerPaymentSummaryTemplate
            (StringBuilder orderDetails, Core.Models.Profile.Customer cust, decimal paymentSum)
        {
            // the following appends a summation of payments on the customer
            MessageTemplateModel footerCustomerTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTERCUSTOMER);
            orderDetails.Append(footerCustomerTemplate.Body.Inject(new
            {
                CustomerNumber = cust.CustomerNumber,
                CustomerBranch = cust.CustomerBranch,
                CustomerName = cust.CustomerName,
                CustomerSum = paymentSum
            }));
        }

        private void ApplyGrandPaymentSummaryTemplate(StringBuilder orderDetails, decimal grandSum, DateTime submittedDate)
        {
            MessageTemplateModel footerGrandTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTERGRAND);
            orderDetails.Append(footerGrandTemplate.Body.Inject(new
            {
                GrandSum = grandSum,
                ScheduledDate = submittedDate
            }));
        }

        private void ApplyEndFooterTemplate(StringBuilder orderDetails)
        {
            MessageTemplateModel footerEndTemplate = _messageTemplateLogic.ReadForKey
                (Constants.MESSAGE_TEMPLATE_MULTI_PAYMENTFOOTEREND);

            orderDetails.Append(footerEndTemplate.Body);
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
                string payerEmail = confirmation.SubmittedBy;
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

        private string GetBankName(Core.Models.OnlinePayments.Customer.EF.CustomerBank bank)
        {
            string ret = BANK_RESOLVE_UNDEFINED;
            if (bank != null && bank.Name != null)
            {
                ret = bank.Name;
            }
            return ret;
        }

        private string GetBankAccountNumber(Core.Models.OnlinePayments.Customer.EF.CustomerBank bank)
        {
            string ret = BANK_RESOLVE_UNDEFINED;
            if (bank != null && bank.AccountNumber != null)
            {
                ret = bank.Name;
            }
            return ret;
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