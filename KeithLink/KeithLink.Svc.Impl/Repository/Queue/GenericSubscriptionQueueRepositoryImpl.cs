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
        private bool _allowProcessing;
        #endregion

        #region constructor
        public GenericSubscriptionQueueRepositoryImpl() {
            _allowProcessing = true;
        }
        #endregion

        #region functions
        public void Subscribe(ConnectionFactory connection, string queue) {
            if (connection.RequestedHeartbeat < 1)
                connection.RequestedHeartbeat = 20;

            using (IConnection c = connection.CreateConnection()) {
                using (IModel channel = c.CreateModel()) {
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                    consumer.Received += MessageReceived;

                    string consumerTag = channel.BasicConsume(queue, false, String.Format("{0}-{1}", Configuration.ApplicationName, queue), consumer);

                    while (_allowProcessing == true) {
                        System.Threading.Thread.Sleep(2000);
                    }

                    consumer.Received -= MessageReceived;

                    consumer.Model.BasicCancel(consumerTag);
                    channel.Close();
                    c.Close();
                }
            }
        }

        public void Unsubscribe() {
            _allowProcessing = false;
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
