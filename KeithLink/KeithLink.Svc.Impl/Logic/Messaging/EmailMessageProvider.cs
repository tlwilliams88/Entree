using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Provider;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Impl.Logic.Messaging {
    public class EmailMessageProvider : IMessageProvider {
        #region attributes
        protected IEmailClient emailClient;
        protected IEventLogRepository eventLogRepository;
        #endregion

        #region ctor
        public EmailMessageProvider(IEmailClient emailClient, IEventLogRepository eventLogRepository) {
            this.emailClient = emailClient;
            this.eventLogRepository = eventLogRepository;
        }
        #endregion

        #region methods
        public void SendMessage(List<Recipient> recipients, Message message) {
            if(recipients == null)
                return;

            foreach(var recipient in recipients) {
                try {
                    emailClient.SendEmail(new List<string>() { recipient.ProviderEndpoint }, null, null, message.MessageSubject, message.MessageBody, message.BodyIsHtml);
                } catch(Exception ex) {
                    eventLogRepository.WriteErrorLog("EmailMessageProvider: Error sending email", ex);
                }
            }
        }
        #endregion
    }
}
