using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Extensions.Enumerations;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
            if (Configuration.OrderServiceMakeKDOELogFiles == null)
            {
                throw new Exception("AppSetting OrderServiceMakeKDOELogFiles is not set.");
            }
            if (Configuration.OrderServiceMakeKDOELogFiles.Equals(Configuration.TRUE_ORDER_SERVICE_MAKEKDOELOGFILES))
            {
                listenForQueueMessagesTask = Task.Factory.StartNew(() => ListenToQueueInTaskForUsers(),
                    CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
        }

        protected void ListenToQueueInTaskForUsers()
        {
            if (Configuration.OrderServiceKDOELogPath == null)
            {
                throw new Exception("AppSetting OrderServiceKDOELogPath is not set.");
            }
            // recursively create directories for saving order history if they don't exist
            Directory.CreateDirectory(Configuration.OrderServiceKDOELogPath);

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
            if (Configuration.OrderServiceKDOELogExtension == null)
            {
                throw new Exception("AppSetting OrderServiceKDOELogExtension is not set.");
            }
            while (consumingMessages && doListenForMessagesInTask)
            {
                string msg = ConsumeMessageFromQueue();

                if (msg != null)
                {
                    eventLogRepository.WriteInformationLog("Processing from queue. OrderHistory: {QueueMessage}".InjectSingleValue("QueueMessage", msg));

                    OrderFile order = JsonConvert.DeserializeObject<OrderFile>(msg);

                    string json = JsonConvert.SerializeObject(order);

                    System.IO.File.WriteAllText
                        (Path.Combine(Configuration.OrderServiceKDOELogPath, 
                                      string.Format("{0}{1}.{2}", Configuration.OrderServiceKDOELogPre, 
                                                                  order.Header.ControlNumber.ToString("D7"), 
                                                                  Configuration.OrderServiceKDOELogExtension)), 
                         json);
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
            if (listenForQueueMessagesTask != null && doListenForMessagesInTask == true)
            {
                doListenForMessagesInTask = false;
                listenForQueueMessagesTask.Wait();
            }
            if (listenForQueueMessagesTask != null)
            {
                eventLogRepository.WriteWarningLog(string.Format("OrderHistoryWriterImpl.listenForQueueMessagesTask.status = {0:G}", listenForQueueMessagesTask.Status));
            }
        }
    }
}
