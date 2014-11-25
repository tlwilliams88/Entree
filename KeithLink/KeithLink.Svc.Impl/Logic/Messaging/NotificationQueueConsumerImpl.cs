using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Enumerations.Messaging;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class NotificationQueueConsumerImpl : INotificationQueueConsumer
    {
        #region attributes
        private Task listenForQueueMessagesTask;
        private bool doListenForMessagesInTask = true;

        private readonly IGenericQueueRepository genericQueueRepository;
        private readonly IEventLogRepository eventLogRepository;
        #endregion

        #region constructor
        public NotificationQueueConsumerImpl(IEventLogRepository eventLogRepository, IGenericQueueRepository genericQueueRepository)
        {
            this.eventLogRepository = eventLogRepository;
            this.genericQueueRepository = genericQueueRepository;
        }
        #endregion
        public void ListenForNotificationMessagesOnQueue()
        {
           listenForQueueMessagesTask = Task.Factory.StartNew(() => ListenToQueueInTask());
        }

        protected void ListenToQueueInTask()
        {
            while (doListenForMessagesInTask)
            {
                try
                {
                    System.Threading.Thread.Sleep(2000);
                    string msg = ConsumeMessageFromQueue();
                    if (msg != null)
                    {
                        BaseNotification notification = NotificationExtension.Deserialize(msg);
                        INotificationHandler handler = GetHandler(notification);
                        handler.ProcessNotification(notification);
                    }
                }
                catch (Exception ex)
                {
                    eventLogRepository.WriteErrorLog("Exception while listening for notifications", ex);
                }
            }
        }

        public INotificationHandler GetHandler(BaseNotification notification)
        {
            throw new NotImplementedException();
        }

        public string ConsumeMessageFromQueue()
        {
            return this.genericQueueRepository.ConsumeFromQueue(Configuration.RabbitMQNotificationServer, Configuration.RabbitMQNotificationUserNameConsumer,
                Configuration.RabbitMQNotificationUserPasswordConsumer, Configuration.RabbitMQVHostNotification, Configuration.RabbitMQQueueNotification);
        }

        public void Stop()
        {
            if (listenForQueueMessagesTask != null && doListenForMessagesInTask == true)
            {
                doListenForMessagesInTask = false;
                listenForQueueMessagesTask.Wait();
            }
        }
    }
}
