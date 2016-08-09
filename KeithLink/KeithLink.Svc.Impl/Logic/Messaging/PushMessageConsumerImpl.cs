using KeithLink.Svc.Core.Interface.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Messaging.Queue;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Common.Core.Interfaces.Logging;
using System.Threading;
using Newtonsoft.Json;
using KeithLink.Common.Core.Extensions;

namespace KeithLink.Svc.Impl.Logic.Messaging
{
    public class PushMessageConsumerImpl : IPushMessageConsumer
    {
        #region attributes
        private const int TWO_SECOND_DELAY = 10000;

        private Task listenForQueueMessagesTask;
        private bool doListenForMessagesInTask = true;
        private bool consumingMessages = false;

        private readonly IGenericQueueRepository _genericQueueRepository;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IPushNotificationMessageProvider _pushMessageProvider;
        #endregion
        #region constructor
        public PushMessageConsumerImpl(IEventLogRepository eventLogRepository, 
            IGenericQueueRepository genericQueueRepository, IPushNotificationMessageProvider pushMessageProvider)
        {
            _eventLogRepository = eventLogRepository;
            _genericQueueRepository = genericQueueRepository;
            _pushMessageProvider = pushMessageProvider;
        }
        #endregion
        public void ListenForQueueMessages()
        {
            listenForQueueMessagesTask = Task.Factory.StartNew(() => ListenToQueueInTaskForUsers(),
                CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public bool ProcessIncomingPushMessage(PushMessage pushmessage)
        {
            // Send message through provider
            _pushMessageProvider.SendMessage(pushmessage.Recipients, pushmessage.Message);
            return true;
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
                    KeithLink.Common.Impl.Email.ExceptionEmail.Send(ex, subject: "Exception processing Push Messages in Queue Service");

                    _eventLogRepository.WriteErrorLog("Exception while listening for push messages", ex);
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
                    PushMessage pushmessage = JsonConvert.DeserializeObject<PushMessage>(msg);

                    _eventLogRepository.WriteInformationLog("Processing push message from queue. Notification: {QueueMessage}".InjectSingleValue("QueueMessage", msg));

                    ProcessIncomingPushMessage(pushmessage);
                }
                else
                {
                    consumingMessages = false;
                }
            }
        }

        public string ConsumeMessageFromQueue()
        {
            if (Configuration.RabbitMQPushMessagesQueue == null)
            {
                throw new Exception("AppSetting RabbitMQPushMessagesQueue is not set.");
            }
            return
                _genericQueueRepository.ConsumeFromQueue(Configuration.RabbitMQNotificationServer, Configuration.RabbitMQNotificationUserNameConsumer,
                Configuration.RabbitMQNotificationUserPasswordConsumer, Configuration.RabbitMQVHostNotification, Configuration.RabbitMQPushMessagesQueue);
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
                //_eventLogRepository.WriteWarningLog(string.Format("OrderHistoryWriterImpl.listenForQueueMessagesTask.status = {0:G}", listenForQueueMessagesTask.Status));
            }
        }

    }
}
