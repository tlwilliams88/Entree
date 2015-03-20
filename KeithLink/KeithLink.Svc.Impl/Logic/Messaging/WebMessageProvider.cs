using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Provider;
using KeithLink.Svc.Impl.Repository.EF.Operational;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class WebMessageProvider : IMessageProvider
    {
        IUserMessageRepository userMessageRepository;
        protected Common.Core.Logging.IEventLogRepository eventLogRepository;
        IUnitOfWork unitOfWork;

        public WebMessageProvider(IUserMessageRepository userMessageRepository, Common.Core.Logging.IEventLogRepository eventLogRepository, IUnitOfWork unitOfWork)
        {
            this.userMessageRepository = userMessageRepository;
            this.eventLogRepository = eventLogRepository;
            this.unitOfWork = unitOfWork;
        }

		public void SendMessage(List<Recipient> recipients, Message message)
        {
			if (recipients == null)
				return;

            foreach (var recipient in recipients)
            {
                try
                {
                    userMessageRepository.Create(
                        new Core.Models.Messaging.EF.UserMessage()
                        {
                            Body = message.MessageBody,
                            CustomerNumber = message.CustomerNumber,
                            Subject = message.MessageSubject,
                            NotificationType = message.NotificationType,
                            UserId = recipient.UserId,
                            Label = message.NotificationType.ToString() // TODO: add a label for the message?
                        });
                }
                catch (Exception ex)
                {
                    eventLogRepository.WriteErrorLog("WebMessageProvider: Error Sending Message", ex);
                }
            }
            unitOfWork.SaveChanges();
        }
    }
}
