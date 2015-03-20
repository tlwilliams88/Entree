using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Core.Interface.Email;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class EmailMessageProvider : IMessageProvider
    {
        protected IEmailClient emailClient;
        protected Common.Core.Logging.IEventLogRepository eventLogRepository;

        public EmailMessageProvider(IEmailClient emailClient, Common.Core.Logging.IEventLogRepository eventLogRepository)
        {
            this.emailClient = emailClient;
            this.eventLogRepository = eventLogRepository;
        }

		public void SendMessage(List<Recipient> recipients, Message message)
        {
			if (recipients == null)
				return;

            foreach (var recipient in recipients)
            {
                try
                {
                    emailClient.SendEmail(new List<string>() { recipient.ProviderEndpoint }, null, null, message.MessageSubject, message.MessageBody, message.BodyIsHtml);
                }
                catch (Exception ex)
                {
                    eventLogRepository.WriteErrorLog("EmailMessageProvider: Error sending email", ex);
                }
            }
        }
    }
}
