using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Configuration;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.UserFeedback;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class UserFeedbackNotificationHandlerImpl : INotificationHandler
    {
        #region attributes
        private readonly IMessageTemplateLogic _messageTemplateLogic;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmailClient _emailClient;
        private readonly IEventLogRepository _eventLogRepository;

        private const string MESSAGE_TEMPLATE_USERFEEDBACK = "UserFeedbackNotice";
        #endregion

        #region ctor
        public UserFeedbackNotificationHandlerImpl(
            IMessageTemplateLogic messageTemplateLogic,
            ICustomerRepository customerRepository,
            IEmailClient emailClient,
            IEventLogRepository eventLogRepository 
            )
        {
            _messageTemplateLogic = messageTemplateLogic;
            _customerRepository = customerRepository;
            _emailClient = emailClient;
            _eventLogRepository = eventLogRepository;
        }
        #endregion

        #region methods
        private Message CreateEmailMessageForNotification(UserFeedbackNotification notification)
        {
            Message message = new Message();
            MessageTemplateModel template = _messageTemplateLogic.ReadForKey(MESSAGE_TEMPLATE_USERFEEDBACK);

            var context = notification.Context;
            var userFeedback = notification.UserFeedback;

            message.MessageSubject = template.Subject.Inject(new
            {
                CustomerNumber = context.CustomerNumber,
                CustomerName = context.CustomerName,
                Audience = userFeedback.Audience.ToString(),
            });

            Customer customer = _customerRepository.GetCustomerByCustomerNumber(notification.CustomerNumber, notification.BranchId);

            StringBuilder header = _messageTemplateLogic.BuildHeader("Feedback from ", customer);

            message.MessageBody += template.Body.Inject(new
            {
                NotifHeader = header.ToString(),

                UserFirstName = context.UserFirstName,
                UserLastName = context.UserLastName,
                SourceEmailAddress = context.SourceEmailAddress,
                SalesRepName = context.SalesRepName,

                Subject = userFeedback.Subject,
                Content = userFeedback.Content,
            });

            message.BodyIsHtml = template.IsBodyHtml;
            message.CustomerNumber = context.CustomerNumber;
            message.CustomerName = context.CustomerName;
            message.BranchId = context.BranchId;
            message.NotificationType = NotificationType.UserFeedback;

            return message;
        }

        public void ProcessNotification(BaseNotification notification)
        {
            try
            {
                if (notification.NotificationType != NotificationType.UserFeedback)
                    throw new ApplicationException("notification/handler type mismatch");

                UserFeedbackNotification feedbackNotification = (UserFeedbackNotification)notification;
                UserFeedbackContext context = feedbackNotification.Context;

                Message message = CreateEmailMessageForNotification(feedbackNotification);

                try
                {
                    _emailClient.SendEmail(
                        new List<string>() { context.TargetEmailAddress },
                        null,
                        null,
                        context.SourceEmailAddress,
                        message.MessageSubject,
                        message.MessageBody,
                        message.BodyIsHtml
                        );
                }
                catch (Exception ex)
                {
                    _eventLogRepository.WriteErrorLog("EmailMessageProvider: Error sending email", ex);
                }

            }
            catch (Exception ex)
            {
                throw new Core.Exceptions.Queue.QueueDataError<BaseNotification>(notification, "UserFeedback:ProcessNotification", "Sending user feedback", "There was an exception processing user feedback", ex);
            }
        }

        #endregion
    }
}
