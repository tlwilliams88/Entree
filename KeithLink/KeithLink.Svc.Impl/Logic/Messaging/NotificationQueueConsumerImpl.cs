using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

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
using KeithLink.Svc.Core;
using System.Threading;
using KeithLink.Svc.Impl.Tasks;

namespace KeithLink.Svc.Impl.Logic.Messaging {
    public class NotificationQueueConsumerImpl : INotificationQueueConsumer {
        #region attributes
        private const int TWO_SECOND_DELAY = 10000;

        private Task listenForQueueMessagesTask;
        private bool doListenForMessagesInTask = true;
        private bool consumingMessages = false;

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
            listenForQueueMessagesTask = Task.Factory.StartNew(() => ListenToQueueInTaskForUsers(), 
                CancellationToken.None, TaskCreationOptions.DenyChildAttach, 
                new LimitedConcurrencyLevelTaskScheduler(Constants.LIMITEDCONCURRENCYTASK_NOTIFICATIONS) );
        }

        protected void ListenToQueueInTaskForUsers() {
            Thread.CurrentThread.IsBackground = true;
            while (doListenForMessagesInTask) {
                consumingMessages = true;

                try
                {
                    ConsumeMessages();
                }
                catch (Exception ex) {
                    consumingMessages = false;
					KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex, subject: "Exception processing Notification in Queue Service");

                    eventLogRepository.WriteErrorLog("Exception while listening for notifications", ex);
                }

                System.Threading.Thread.Sleep(TWO_SECOND_DELAY);
            }
        }

        private void ConsumeMessages()
        {
            while (consumingMessages && doListenForMessagesInTask)
            {
                string msg = ConsumeMessageFromQueue();

                if (msg != null)
                {
                    BaseNotification notification = NotificationExtension.Deserialize(msg);

                    eventLogRepository.WriteInformationLog("Processing notification from queue. Notification: {QueueMessage}".InjectSingleValue("QueueMessage", msg));

                    var handler = notificationHandlerFactory(notification.NotificationType); // autofac will get the right handler
                    handler.ProcessNotification(notification);
                }
                else
                {
                    consumingMessages = false;
                }
            }
        }

        public string ConsumeMessageFromQueue() {
            return 
                KeithLink.Svc.Impl.Helpers.Retry.Do<string>
                (() => this.genericQueueRepository.ConsumeFromQueue(Configuration.RabbitMQNotificationServer, Configuration.RabbitMQNotificationUserNameConsumer,
                Configuration.RabbitMQNotificationUserPasswordConsumer, Configuration.RabbitMQVHostNotification, Configuration.RabbitMQQueueNotification), 
                TimeSpan.FromSeconds(1), Constants.QUEUE_REPO_RETRY_COUNT);
        }


        public void Stop()
        {
            if(listenForQueueMessagesTask != null && doListenForMessagesInTask == true) {
                doListenForMessagesInTask = false;
                listenForQueueMessagesTask.Wait();
            }
            if (listenForQueueMessagesTask != null)
            {
                //eventLogRepository.WriteWarningLog(string.Format("NotificationQueueConsumerImpl.listenForQueueMessagesTask.status = {0:G}", listenForQueueMessagesTask.Status));
            }
        }
        #endregion
    }
}

