using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Extensions.Messaging;
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
using KeithLink.Svc.Impl.Helpers;

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
		private readonly IDsrServiceRepository dsrServiceRepository;
        #endregion

        #region ctor
        public PaymentConfirmationNotificationHandlerImpl(IEventLogRepository eventLogRepository, IUserProfileLogic userProfileLogic, 
                                                          IUserPushNotificationDeviceRepository userPushNotificationDeviceRepository, ICustomerRepository customerRepository, 
                                                          IUserMessagingPreferenceRepository userMessagingPreferenceRepository, Func<Channel, IMessageProvider> messageProviderFactory,
														  IMessageTemplateLogic messageTemplateLogic, IKPayInvoiceRepository kpayInvoiceRepo, ICustomerBankRepository customerBankRepo, IDsrServiceRepository dsrServiceRepository)
            : base(userProfileLogic, userPushNotificationDeviceRepository, customerRepository, 
                   userMessagingPreferenceRepository, messageProviderFactory, eventLogRepository, dsrServiceRepository)
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
        
        private Message GetEmailMessageForNotification(PaymentConfirmationNotification notification, Svc.Core.Models.Profile.Customer customer) {
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_PAYMENTCONFIRMATION);
            MessageTemplateModel detailTemplate = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_PAYMENTDETAIL);

            StringBuilder orderDetails = new StringBuilder();

            foreach(var payment in notification.Payments){
				var invoice = _invoiceRepo.GetInvoiceHeader(DivisionHelper.GetDivisionFromBranchId(notification.BranchId), notification.CustomerNumber, payment.InvoiceNumber);

                orderDetails.Append(detailTemplate.Body.Inject(new { 
                                                                        InvoiceType = invoice.InvoiceType,
                                                                        InvoiceNumber = payment.InvoiceNumber,
                                                                        InvoiceDate = invoice.InvoiceDate,
                                                                        DueDate = invoice.DueDate,
                                                                        ScheduledDate = payment.PaymentDate,
                                                                        PaymentAmount = payment.PaymentAmount
                                                                   }));
            }

			var bank = _bankRepo.GetBankAccount(DivisionHelper.GetDivisionFromBranchId(notification.BranchId), notification.CustomerNumber, notification.Payments[0].AccountNumber);
            var confirmationId = notification.Payments.FirstOrDefault().ConfirmationId;

            Message message = new Message();
            message.BodyIsHtml = template.IsBodyHtml;
			message.MessageSubject = template.Subject.Inject(customer);
            message.MessageBody = template.Body.Inject(new { 
                                                                ConfirmationId = confirmationId,
                                                                CustomerNumber = customer.CustomerNumber,
                                                                CustomerName = customer.CustomerName,
                                                                BranchId = notification.BranchId,
                                                                BankAccount = bank.AccountNumber + " - " + bank.Name,
                                                                PaymentDetailLines = orderDetails.ToString(),
                                                                TotalPayments = notification.Payments.Sum(p => p.PaymentAmount)
                                                           });
            message.CustomerNumber = customer.CustomerNumber;
			message.CustomerName = customer.CustomerName;
			message.BranchId = customer.CustomerBranch;
            message.NotificationType = NotificationType.PaymentConfirmation;
            return message;
        }

        public void ProcessNotification(BaseNotification notification) {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.PaymentConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            // had to setup a translation for this type in Svc.Core.Extensions to deserialize the message with the concrete type
            PaymentConfirmationNotification confirmation = (PaymentConfirmationNotification)notification;
            
            // load up recipients, customer and message
            Svc.Core.Models.Profile.Customer customer = _customerRepo.GetCustomerByCustomerNumber(confirmation.CustomerNumber, confirmation.BranchId);

            if (customer == null) {
                System.Text.StringBuilder warningMessage = new StringBuilder();
                warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Payment Confirmation notification.", notification.BranchId, notification.CustomerNumber);
                warningMessage.AppendLine();
                warningMessage.AppendLine();
                warningMessage.AppendLine("Notification:");
                warningMessage.AppendLine(notification.ToJson());

                _log.WriteWarningLog(warningMessage.ToString());
            } else {
                List<Recipient> recipients = base.LoadRecipients(confirmation.NotificationType, customer);
                Message message = GetEmailMessageForNotification(confirmation, customer);

                // send messages to providers...
                if (recipients != null && recipients.Count > 0) {
                    base.SendMessage(recipients, message);
                }
            }
        }

        public void ProcessNotificationForExternalUsers(BaseNotification notification)
        {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.PaymentConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            // had to setup a translation for this type in Svc.Core.Extensions to deserialize the message with the concrete type
            PaymentConfirmationNotification confirmation = (PaymentConfirmationNotification)notification;

            // load up recipients, customer and message
            Svc.Core.Models.Profile.Customer customer = _customerRepo.GetCustomerByCustomerNumber(confirmation.CustomerNumber, confirmation.BranchId);

            if (customer == null)
            {
                System.Text.StringBuilder warningMessage = new StringBuilder();
                warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Payment Confirmation notification.", notification.BranchId, notification.CustomerNumber);
                warningMessage.AppendLine();
                warningMessage.AppendLine();
                warningMessage.AppendLine("Notification:");
                warningMessage.AppendLine(notification.ToJson());

                _log.WriteWarningLog(warningMessage.ToString());
            }
            else
            {
                List<Recipient> recipients = base.LoadRecipients(confirmation.NotificationType, customer, false, false, true); 
                Message message = GetEmailMessageForNotification(confirmation, customer);

                // send messages to providers...
                if (recipients != null && recipients.Count > 0)
                {
                    try
                    {
                        base.SendMessage(recipients, message);
                    }
                    catch (Exception ex)
                    {
                        _log.WriteErrorLog(String.Format("Error sending messages {0} {1}", ex.Message, ex.StackTrace));
                    } 
                }
            }
        }

        public void ProcessNotificationForInternalUsers(BaseNotification notification)
        {
            if (notification.NotificationType != Core.Enumerations.Messaging.NotificationType.PaymentConfirmation)
                throw new ApplicationException("notification/handler type mismatch");

            // had to setup a translation for this type in Svc.Core.Extensions to deserialize the message with the concrete type
            PaymentConfirmationNotification confirmation = (PaymentConfirmationNotification)notification;

            // load up recipients, customer and message
            Svc.Core.Models.Profile.Customer customer = _customerRepo.GetCustomerByCustomerNumber(confirmation.CustomerNumber, confirmation.BranchId);

            if (customer == null)
            {
                System.Text.StringBuilder warningMessage = new StringBuilder();
                warningMessage.AppendFormat("Could not find customer({0}-{1}) to send Payment Confirmation notification.", notification.BranchId, notification.CustomerNumber);
                warningMessage.AppendLine();
                warningMessage.AppendLine();
                warningMessage.AppendLine("Notification:");
                warningMessage.AppendLine(notification.ToJson());

                _log.WriteWarningLog(warningMessage.ToString());
            }
            else
            {
                List<Recipient> recipients = base.LoadRecipients(confirmation.NotificationType, customer, false, true, false);
                Message message = GetEmailMessageForNotification(confirmation, customer);

                // send messages to providers...
                if (recipients != null && recipients.Count > 0)
                {
                    try
                    {
                        base.SendMessage(recipients, message);
                    }
                    catch (Exception ex)
                    {
                        _log.WriteErrorLog(String.Format("Error sending messages {0} {1}", ex.Message, ex.StackTrace));
                    } 
                }
            }
        }
        
        #endregion
    }
}
