using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Orders
{
    public class OrderQueueRepositoryImpl : IQueueRepository {

        #region attributes
        private IEventLogRepository _log;
        #endregion

        #region ctor
        public OrderQueueRepositoryImpl(IEventLogRepository EventLog) {
            _log = EventLog;
        }
        #endregion

        #region methods
        public void AcknowledgeReceipt(ulong DeliveryTag) {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQOrderServer,
                UserName = Configuration.RabbitMQOrderConsumerUserName,
                Password = Configuration.RabbitMQOrderConsumerUserPassword,
                VirtualHost = Configuration.RabbitMQOrderVHost
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {
                    model.BasicAck(DeliveryTag, false);
                }
            }
        }

        public QueueReturn ConsumeFromQueue() {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQOrderServer,
                UserName = Configuration.RabbitMQOrderConsumerUserName,
                Password = Configuration.RabbitMQOrderConsumerUserPassword,
                VirtualHost = Configuration.RabbitMQOrderVHost
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {
                    BasicGetResult result = model.BasicGet(Configuration.RabbitMQOrderQueue, false);

                    if (result == null) {
                        return null;
                    } else {
                        return new QueueReturn() {
                            DeliveryTag = result.DeliveryTag,
                            Message = Encoding.UTF8.GetString(result.Body)
                        };
                    }
                }
            }
        }

        public void DenyReceipt(ulong DeliveryTag) {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQOrderServer,
                UserName = Configuration.RabbitMQOrderConsumerUserName,
                Password = Configuration.RabbitMQOrderConsumerUserPassword,
                VirtualHost = Configuration.RabbitMQOrderVHost
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {
                    model.BasicNack(DeliveryTag, false, true);
                }
            }
        }

        public void PublishToQueue(string item) {
            // this connection uses different credentials than the other methods in this class
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQOrderServer,
                UserName = Configuration.RabbitMQOrderPublisherUserName,
                Password = Configuration.RabbitMQOrderPublisherUserPassword,
                VirtualHost = Configuration.RabbitMQOrderVHost
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {
                    model.QueueBind(Configuration.RabbitMQOrderQueue, Configuration.RabbitMQExchangeName, string.Empty, new Dictionary<string, object>());

                    IBasicProperties props = model.CreateBasicProperties();
                    props.DeliveryMode = 2; // persistent delivery mode

                    model.BasicPublish(Configuration.RabbitMQExchangeName, string.Empty, false, props, Encoding.UTF8.GetBytes(item));
                }
            }
        }

        #endregion
    }
}
