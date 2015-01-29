using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.Email;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging {
    public class PaymentConfirmationNotificationHandlerImpl : BaseNotificationHandlerImpl, INotificationHandler {
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
                                                          IMessageTemplateLogic messageTemplateLogic, IKPayInvoiceRepository kpayInvoiceRepo, ICustomerBankRepository customerBankRepo)
            : base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository, 
                   userMessagingPreferenceRepository, messageProviderFactory)
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
        private string GetDivision(string branchId) {
            if (branchId.Length == 5) {
                return branchId;
            } else if (branchId.Length == 3) {
                switch (branchId.ToUpper()) {
                    case "FAM":
                        return "FAM04";
                    case "FAQ":
                        return "FAQ08";
                    case "FAR":
                        return "FAR09";
                    case "FDF":
                        return "FDF01";
                    case "FHS":
                        return "FHS03";
                    case "FLR":
                        return "FLR05";
                    case "FOK":
                        return "FOK06";
                    case "FSA":
                        return "FSA07";
                    default:
                        return null;
                }
            } else {
                return null;
            }
        }

        private Message GetEmailMessageForNotification(PaymentConfirmationNotification notification, Svc.Core.Models.Profile.Customer customer) {
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_PAYMENTCONFIRMATION);
            MessageTemplateModel detailTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_PAYMENTDETAIL);

            StringBuilder orderDetails = new StringBuilder();

            foreach(var payment in notification.Payments){
                var invoice = _invoiceRepo.GetInvoiceHeader(GetDivision(notification.BranchId), notification.CustomerNumber, payment.InvoiceNumber);

                orderDetails.Append(detailTemplate.Body.Inject(new { 
                                                                        InvoiceType = invoice.InvoiceType,
                                                                        InvoiceNumber = payment.InvoiceNumber,
                                                                        InvoiceDate = invoice.InvoiceDate,
                                                                        DueDate = invoice.DueDate,
                                                                        ScheduledDate = payment.PaymentDate,
                                                                        PaymentAmount = payment.PaymentAmount
                                                                   }));
            }

            var bank = _bankRepo.GetBankAccount(GetDivision(notification.BranchId), notification.CustomerNumber, notification.Payments[0].AccountNumber);

            Message message = new Message();
            message.MessageSubject = template.Subject.Inject(customer);
            message.MessageBody = template.Body.Inject(new { 
                                                                CustomerNumber = customer.CustomerNumber,
                                                                CustomerName = customer.CustomerName,
                                                                BranchId = notification.BranchId,
                                                                BankAccount = bank.AccountNumber + " - " + bank.Name,
                                                                PaymentDetailLines = orderDetails.ToString(),
                                                                TotalPayments = notification.Payments.Sum(p => p.PaymentAmount)
                                                           });
            message.CustomerNumber = customer.CustomerNumber;
            message.NotificationType = NotificationType.PaymentConfirmation;
            return message;
        }

        public void ProcessNotification(BaseNotification notification) {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.PaymentConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            // had to setup a translation for this type in Svc.Core.Extensions to deserialize the message with the concrete type
            PaymentConfirmationNotification confirmation = (PaymentConfirmationNotification)notification;
            
            // load up recipients, customer and message
            Svc.Core.Models.Profile.Customer customer = _customerRepo.GetCustomerByCustomerNumber(confirmation.CustomerNumber);
            List<Recipient> recipients = base.LoadRecipients(confirmation.NotificationType, customer);
            Message message = GetEmailMessageForNotification(confirmation, customer);

            // send messages to providers...
            base.SendMessage(recipients, message);
        }
        #endregion
    }
}
