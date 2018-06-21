using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Interface.UserFeedback;
using KeithLink.Svc.Core.Models.UserFeedback;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Net.Mail;

namespace KeithLink.Svc.Impl.Logic.UserFeedback
{
    public class UserFeedbackLogicImpl : IUserFeedbackLogic
    {
        #region attributes
        private readonly IUserFeedbackRepository _userFeedbackRepo;
        private readonly IEventLogRepository _eventLog;
        //private readonly IEmailClient _emailClient;
        #endregion

        #region ctor
        public UserFeedbackLogicImpl(
                                    IUserFeedbackRepository userFeedbackRepo,
                                    IEventLogRepository eventLog
                                    //IEmailClient emailClient
                                    )
        {
            _userFeedbackRepo = userFeedbackRepo;
            _eventLog = eventLog;
            //_emailClient = emailClient;
        }
        #endregion

        #region methods

        /// <summary>
        /// validate all of the attributes of the user's feedback
        /// </summary>
        /// <remarks>
        /// </remarks>
        private void AssertUserFeedback(Core.Models.UserFeedback.UserFeedback userFeedback)
        {
            if (userFeedback.Subject.Length == 0)
                throw new ApplicationException("Subject is blank.");

            if (userFeedback.Content.Length == 0)
                throw new ApplicationException("Content is blank.");

            if (Enum.IsDefined(typeof(Audience), userFeedback.Audience) == false)
                throw new ApplicationException("Audience value is invalid.");

        }


        /// <summary>
        /// Saves user feedback and forwards feedback to target audience.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userFeedback"></param>
        /// <returns></returns>
        public int SubmitUserFeedback(UserFeedbackContext context, Core.Models.UserFeedback.UserFeedback userFeedback)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (userFeedback == null)
                throw new ArgumentNullException("userFeedback");

            AssertUserFeedback(userFeedback);

            //List<Recipient> recipients = new List<Recipient>();
            //var recipient = new Recipient()
            //{
            //    UserId = user.UserId,
            //    UserEmail = "entreefeedback@benekeith.com",

            //    CustomerNumber = user.CustomerNumber,
            //    Channel = Core.Enumerations.Messaging.Channel.Email,
            //};

            //recipients.Add(recipient);
            //var message = new Message()
            //{
            //    MessageSubject = userFeedback.Subject,
            //    MessageBody = userFeedback.Content,
            //};

            //_emailMessageProvider.SendMessage(recipients, message);

           MailMessage message = new MailMessage(context.SourceEmailAddress, context.TargetEmailAddress, userFeedback.Subject, userFeedback.Content);
            //SmtpClient client = new SmtpClient(mailServer);
            //client.UseDefaultCredentials = true;
            //client.Send(message);


            int rowsAffected = _userFeedbackRepo.SaveUserFeedback(context, userFeedback);

            return rowsAffected;
        }

        #endregion
    }
}
