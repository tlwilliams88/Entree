using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.Orders
{
    public class OrderHistoryWriterImpl : IOrderHistoryWriter
    {
        #region attributes
        private const int TWO_SECOND_DELAY = 10000;

        private Task listenForQueueMessagesTask;
        private bool doListenForMessagesInTask = true;
        private bool consumingMessages = false;

        private readonly IGenericQueueRepository genericQueueRepository;
        private readonly IEventLogRepository eventLogRepository;
        //Func<NotificationType, INotificationHandler> notificationHandlerFactory;
        #endregion
        #region constructor
        public OrderHistoryWriterImpl(IEventLogRepository eventLogRepository, IGenericQueueRepository genericQueueRepository)
        {
            this.eventLogRepository = eventLogRepository;
            this.genericQueueRepository = genericQueueRepository;
        }
        #endregion
        public void ListenForNotificationMessagesOnQueue()
        {
            listenForQueueMessagesTask = Task.Factory.StartNew(() => ListenToQueueInTaskForUsers(),
                CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        protected void ListenToQueueInTaskForUsers()
        {
            while (doListenForMessagesInTask)
            {
                consumingMessages = true;

                try
                {
                    ConsumeMessages();
                }
                catch (Exception ex)
                {
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
                    eventLogRepository.WriteInformationLog("Processing notification from queue. OrderHistory: {QueueMessage}".InjectSingleValue("QueueMessage", msg));
                }
                else
                {
                    consumingMessages = false;
                }
            }
        }

        public string ConsumeMessageFromQueue()
        {
            return
                this.genericQueueRepository.ConsumeFromQueue(Configuration.RabbitMQOrderServer, Configuration.RabbitMQUserNameConsumer,
                Configuration.RabbitMQUserPasswordConsumer, Configuration.RabbitMQVHostOrder, Configuration.RabbitMQQueueOrderHistory);
        }

        public void Stop()
        {
            //if (listenForQueueMessagesTask != null && doListenForMessagesInTask == true)
            //{
            //    doListenForMessagesInTask = false;
            //    listenForQueueMessagesTask.Wait();
            //}
            //if (listenForQueueMessagesTask != null)
            //{
            //    eventLogRepository.WriteWarningLog(string.Format("NotificationQueueConsumerImpl.listenForQueueMessagesTask.status = {0:G}", listenForQueueMessagesTask.Status));
            //}
        }
    }
}
