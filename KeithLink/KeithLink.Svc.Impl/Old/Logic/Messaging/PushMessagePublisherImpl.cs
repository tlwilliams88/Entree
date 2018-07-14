using Entree.Core.Interface.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Models.Messaging.EF;
using Entree.Core.Models.Messaging.Provider;
using Newtonsoft.Json;
using Entree.Core.Interface.Common;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class PushMessagePublisherImpl : IPushNotificationMessageProvider
    {
        private IGenericQueueRepository _queueRepository;
        public PushMessagePublisherImpl(IGenericQueueRepository queueRepository)
        {
            _queueRepository = queueRepository;
        }
        public string RegisterRecipient(UserPushNotificationDevice userPushNotificationDevice)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(List<Recipient> recipients, Message message)
        {
            if(Configuration.RabbitMQPushMessagesExchange == null)
            {
                throw new Exception("AppSetting RabbitMQPushMessagesExchange is not set.");
            }
            // encapsulate recipients and message in serializable message
            Core.Models.Messaging.Queue.PushMessage pushNotification = new Core.Models.Messaging.Queue.PushMessage();
            pushNotification.CreateDate = DateTime.Now;
            pushNotification.Recipients = recipients;
            pushNotification.Message = message;

            // publish that message to push messages exchange
#if !DEBUG
            _queueRepository.PublishToQueue(JsonConvert.SerializeObject(pushNotification), Configuration.RabbitMQNotificationServer,
                Configuration.RabbitMQNotificationUserNamePublisher, Configuration.RabbitMQNotificationUserPasswordPublisher,
                Configuration.RabbitMQVHostNotification, Configuration.RabbitMQPushMessagesExchange);
#endif
        }
    }
}
