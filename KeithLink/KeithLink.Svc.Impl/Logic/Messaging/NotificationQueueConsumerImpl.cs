using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Exceptions.Queue;
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Messaging;
using KeithLink.Svc.Core.Models.Messaging.Queue;

using KeithLink.Svc.Impl.Repository.EF.Operational;

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
        private readonly IGenericSubsriptionQueueRepository genericSubscriptionQueue;
        private readonly IEventLogRepository eventLogRepository;
        Func<NotificationType, INotificationHandler> notificationHandlerFactory;

        public string RabbitMQQueueName { get; set; }
        private IUnitOfWork _uow;

        #endregion

        #region constructor
        public NotificationQueueConsumerImpl(IEventLogRepository eventLogRepository, IGenericQueueRepository genericQueueRepository,
            Func<NotificationType, INotificationHandler> notificationHandlerFactory, IUnitOfWork unitOfWork, 
            IGenericSubsriptionQueueRepository genericSubscriptionQueue) {

            this.eventLogRepository = eventLogRepository;
            this.genericQueueRepository = genericQueueRepository;
            this.genericSubscriptionQueue = genericSubscriptionQueue;
            this.notificationHandlerFactory = notificationHandlerFactory;
            this._uow = unitOfWork;

            this.genericSubscriptionQueue.MessageReceived += GenericSubscriptionQueue_MessageReceived;

        }

        #endregion

        #region polling
        public void ListenForNotificationMessagesOnQueue() {
            listenForQueueMessagesTask = Task.Factory.StartNew(() => ListenToQueueInTaskForUsers(), 
                CancellationToken.None, TaskCreationOptions.DenyChildAttach, 
                new LimitedConcurrencyLevelTaskScheduler(Constants.LIMITEDCONCURRENCYTASK_NOTIFICATIONS) );
        }

        protected void ListenToQueueInTaskForUsers() {
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

                    // Always clear the context at the end of a transaction
                    _uow.ClearContext();
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
                Configuration.RabbitMQNotificationUserPasswordConsumer, Configuration.RabbitMQVHostNotification, RabbitMQQueueName), 
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

        #region subscription

        public void SubscribeToQueue(string queue) {
            RabbitMQ.Client.ConnectionFactory config = new RabbitMQ.Client.ConnectionFactory();
            config.HostName = Configuration.RabbitMQNotificationServer;
            config.UserName = Configuration.RabbitMQNotificationUserNameConsumer;
            config.Password = Configuration.RabbitMQNotificationUserPasswordConsumer;
            config.VirtualHost = Configuration.RabbitMQVHostNotification;

            eventLogRepository.WriteInformationLog(string.Format("Subscribing to notification queue: {0}",  queue));

            this.listenForQueueMessagesTask = Task.Factory.StartNew(() => genericSubscriptionQueue.Subscribe(config, queue),
                CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                new LimitedConcurrencyLevelTaskScheduler(Constants.LIMITEDCONCURRENCYTASK_NOTIFICATIONS));
        }

        public void Unsubscribe() {
            genericSubscriptionQueue.Unsubscribe();
        }

        private void GenericSubscriptionQueue_MessageReceived(RabbitMQ.Client.IBasicConsumer sender, RabbitMQ.Client.Events.BasicDeliverEventArgs args) {
            RabbitMQ.Client.Events.EventingBasicConsumer consumer = (RabbitMQ.Client.Events.EventingBasicConsumer)sender;

            try {
                BaseNotification notification = NotificationExtension.Deserialize(Encoding.ASCII.GetString(args.Body));

                eventLogRepository.WriteInformationLog("Processing notification type: {NotificationType}. Data: {QueueMessage}".Inject(new { QueueMessage = notification.ToJson(), NotificationType = notification.NotificationType.ToString() }));

                var handler = notificationHandlerFactory(notification.NotificationType); // autofac will get the right handler
                handler.ProcessNotification(notification);

                // Always clear the context at the end of a transaction
                _uow.ClearContext();

                genericSubscriptionQueue.Ack(consumer, args.DeliveryTag);
            } catch (QueueDataError<string> serializationEx)  {
                eventLogRepository.WriteErrorLog("Serializing problem with notification.", serializationEx);
            } catch (QueueDataError<BaseNotification> notificationEx) {
                eventLogRepository.WriteErrorLog("Error processing notification.", notificationEx);

                PublishToQueue(notificationEx.ProcessingObject, Configuration.RabbitMQNotificationErrorQueue);
            } catch (Exception ex) {
                eventLogRepository.WriteErrorLog("Unhandled error processing notification.", ex);
            }
        }

        private void PublishToQueue(BaseNotification notification, string queue) {
            string serializedNotification = Newtonsoft.Json.JsonConvert.SerializeObject(notification);

            genericQueueRepository.PublishToQueue(serializedNotification, Configuration.RabbitMQNotificationServer, Configuration.RabbitMQNotificationUserNameConsumer,
                Configuration.RabbitMQNotificationUserPasswordPublisher, Configuration.RabbitMQVHostNotification, Configuration.RabbitMQExchangeNotificationError);
        }

        #endregion
    }
}

