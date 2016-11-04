using KeithLink.Common.Core.Interfaces.Logging;

using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace KeithLink.Svc.Impl.Repository.Queue {
    public class GenericSubscriptionQueueRepositoryImpl : IGenericSubsriptionQueueRepository {
        #region attributes
        private ConnectionFactory _connection;
        private string _queue;

        private bool _processing;
        private bool _restartProcessing;
        private bool _shutdown;

        private IEventLogRepository _logger;

        #endregion

        #region constructor
        public GenericSubscriptionQueueRepositoryImpl(IEventLogRepository logger) {
            _processing = false;
            _restartProcessing = false;
            _shutdown = false;

            _logger = logger;
        }

        #endregion

        #region functions
        public void Subscribe(ConnectionFactory connection, string queue) {
            if (connection.RequestedHeartbeat < 1)
                connection.RequestedHeartbeat = 20;

            _connection = connection;
            _queue = queue;

            _processing = true;

            StartProcessing();

            while (_shutdown == false) {
                if (_restartProcessing == true && _processing == true) {
                    StartProcessing();
                }
            }
            
        }

        private void StartProcessing() {
            using (IConnection c = _connection.CreateConnection()) {
                using (IModel channel = c.CreateModel()) {
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                    consumer.Received += MessageReceived;

                    string consumerTag = channel.BasicConsume(_queue, false, String.Format("{0}-{1}", Configuration.ApplicationName, _queue), consumer);

                    while (_processing == true && _restartProcessing == false) {
                        // If there was an exception from Rabbit and the channel closes, re-subscribe.
                        if (channel.CloseReason != null) {
                            _restartProcessing = true;
                            break;
                        }

                        System.Threading.Thread.Sleep(2000);
                    }

                    consumer.Received -= MessageReceived;

                    if (channel.IsOpen) {
                        consumer.Model.BasicCancel(consumerTag);
                        channel.Close();
                        c.Close();
                    }
                }
            }

            if (_processing == false) {
                _shutdown = true;
            }
        }

        public void Unsubscribe() {
            _processing = false;
        }

        public void Ack(EventingBasicConsumer consumer, ulong deliveryTag) {
            if (consumer.Model.IsOpen) {
                consumer.Model.BasicAck(deliveryTag, false);
            }
        }

        public void Nack(EventingBasicConsumer consumer, ulong deliveryTag) {
            if (consumer.Model.IsOpen) {
                consumer.Model.BasicNack(deliveryTag, false, true);
            }
        }
        #endregion

        #region events
        public event BasicDeliverEventHandler MessageReceived;
        #endregion

        #region properties
        #endregion

    }
}
