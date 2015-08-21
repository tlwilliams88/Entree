using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Queue;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging {
    public class NotificationQueueConsumerImpl : INotificationQueueConsumer {
        #region attributes
        private Task listenForQueueMessagesTask;
        private bool doListenForMessagesInTask = true;

        private readonly IGenericQueueRepository genericQueueRepository;
        private readonly IEventLogRepository eventLogRepository;
        Func<NotificationType, INotificationHandler> notificationHandlerFactory;
        #endregion

        #region constructor
        public NotificationQueueConsumerImpl(IEventLogRepository eventLogRepository, IGenericQueueRepository genericQueueRepository,
            Func<NotificationType, INotificationHandler> notificationHandlerFactory) {
            this.eventLogRepository = eventLogRepository;
            this.genericQueueRepository = genericQueueRepository;
            this.notificationHandlerFactory = notificationHandlerFactory;
        }
        #endregion

        #region methods
        public void ListenForNotificationMessagesOnQueue() {
            listenForQueueMessagesTask = Task.Factory.StartNew(() => ListenToQueueInTask());
        }

        protected void ListenToQueueInTask() {
            while (doListenForMessagesInTask) {
                try {
                    System.Threading.Thread.Sleep(2000);
                    string msg = ConsumeMessageFromQueue();

					if (msg != null) {
                        BaseNotification notification = NotificationExtension.Deserialize(msg);
                        
                        eventLogRepository.WriteInformationLog("Processing notification from queue. Notification: {QueueMessage}".InjectSingleValue("QueueMessage", msg));

                        var handler = notificationHandlerFactory(notification.NotificationType); // autofac will get the right handler
                        handler.ProcessNotification(notification);
                    }
                } catch (Exception ex) {
					KeithLink.Common.Core.Email.ExceptionEmail.Send(ex, subject: "Exception processing Notification in Queue Service");

                    eventLogRepository.WriteErrorLog("Exception while listening for notifications", ex);
                }
            }
        }

        public string ConsumeMessageFromQueue() {
            return this.genericQueueRepository.ConsumeFromQueue(Configuration.RabbitMQNotificationServer, Configuration.RabbitMQNotificationUserNameConsumer,
                Configuration.RabbitMQNotificationUserPasswordConsumer, Configuration.RabbitMQVHostNotification, Configuration.RabbitMQQueueNotification);
        }

        public void Stop() {
            if (listenForQueueMessagesTask != null && doListenForMessagesInTask == true) {
                doListenForMessagesInTask = false;
                listenForQueueMessagesTask.Wait();
            }
        }
        #endregion
    }
}
