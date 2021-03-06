﻿using KeithLink.Svc.Core.Interface.Messaging;
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
using KeithLink.Svc.Core.Extensions.Messaging;
using KeithLink.Svc.Core.Enumerations.Messaging;
using KeithLink.Svc.Core.Exceptions.Queue;

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
        private readonly IGenericSubscriptionQueueRepository _genericSubscriptionQueue;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IPushNotificationMessageProvider _pushMessageProvider;
        #endregion
        #region constructor
        public PushMessageConsumerImpl(IEventLogRepository eventLogRepository, 
            IGenericSubscriptionQueueRepository genericSubscriptionQueue,
            IGenericQueueRepository genericQueueRepository, IPushNotificationMessageProvider pushMessageProvider)
        {
            _eventLogRepository = eventLogRepository;
            _genericQueueRepository = genericQueueRepository;
            _genericSubscriptionQueue = genericSubscriptionQueue;
            _pushMessageProvider = pushMessageProvider;

            // subscribe to event to receive message through subscription
            _genericSubscriptionQueue.MessageReceived += GenericSubscriptionQueue_MessageReceived;
        }
        #endregion
        #region methods
        #region polling
        public void ListenForQueueMessages()
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
        #endregion

        #region subscription
        public void SubscribeToQueue()
        {
            RabbitMQ.Client.ConnectionFactory config = new RabbitMQ.Client.ConnectionFactory();
            config.HostName = Configuration.RabbitMQNotificationServer;
            config.UserName = Configuration.RabbitMQNotificationUserNameConsumer;
            config.Password = Configuration.RabbitMQNotificationUserPasswordConsumer;
            config.VirtualHost = Configuration.RabbitMQVHostNotification;

            _eventLogRepository.WriteInformationLog
                (string.Format("Subscribing to push messages queue: {0}", Configuration.RabbitMQPushMessagesQueue));

            this.listenForQueueMessagesTask = Task.Factory.StartNew
                (() => _genericSubscriptionQueue.Subscribe(config, Configuration.RabbitMQPushMessagesQueue));
        }

        public void Unsubscribe()
        {
            _genericSubscriptionQueue.Unsubscribe();
        }

        private void GenericSubscriptionQueue_MessageReceived
            (RabbitMQ.Client.IBasicConsumer sender, RabbitMQ.Client.Events.BasicDeliverEventArgs args)
        {
            RabbitMQ.Client.Events.EventingBasicConsumer consumer = (RabbitMQ.Client.Events.EventingBasicConsumer)sender;

            try
            {
                // don't reprocess items that have been processed
                if (_genericSubscriptionQueue.GetLastProcessedUndelivered() != args.DeliveryTag)
                {
                    string msg = Encoding.ASCII.GetString(args.Body);

                    PushMessage pushmessage = JsonConvert.DeserializeObject<PushMessage>(msg);

                    _eventLogRepository.WriteInformationLog
                        ("Processing push message from queue. Notification: {QueueMessage}".InjectSingleValue
                            ("QueueMessage", msg));

                    ProcessIncomingPushMessage(pushmessage);
                }

                _genericSubscriptionQueue.Ack(consumer, args.DeliveryTag);
            }
            catch (QueueDataError<string> serializationEx)
            {
                _eventLogRepository.WriteErrorLog("Serializing problem with push message.", serializationEx);
            }
            catch (QueueDataError<BaseNotification> notificationEx)
            {
                _eventLogRepository.WriteErrorLog("Error processing push message.", notificationEx);
            }
            catch (Exception ex)
            {
                _eventLogRepository.WriteErrorLog("Unhandled error processing push message.", ex);
            }
        }
        #endregion

        public bool ProcessIncomingPushMessage(PushMessage pushmessage)
        {
            NewRelic.Api.Agent.NewRelic.AddCustomParameter("Recipients.Count", pushmessage.Recipients.Count);
            if (pushmessage.Recipients.Count > 0)
            {
                NewRelic.Api.Agent.NewRelic.AddCustomParameter("0-DeviceId", pushmessage.Recipients[0].DeviceId);
                NewRelic.Api.Agent.NewRelic.AddCustomParameter("0-ProviderEndpoint", pushmessage.Recipients[0]
                                                                                                .ProviderEndpoint);
                NewRelic.Api.Agent.NewRelic.AddCustomParameter("0-UserId", JsonConvert.SerializeObject
                                                                            (pushmessage.Recipients[0].UserId));

                // Send message through provider
                _pushMessageProvider.SendMessage(pushmessage.Recipients, pushmessage.Message);
            }
            else
            {
                _eventLogRepository.WriteErrorLog("Push message consumed with 0 recipients.");
            }
            return true;
        }
        #endregion
    }
}
