using KeithLink.Svc.Core.Enumerations.OrderHistory;
using KeithLink.Svc.Core.Interface.Common;
using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KeithLink.Svc.Impl.Repository.Orders.History {
    public class OrderUpdateQueueRepositoryImpl : IQueueRepository {
        #region attributes
        private OrderHistoryQueueLocation _queuePath;
        #endregion

        #region ctor
        public OrderUpdateQueueRepositoryImpl() {
            _queuePath = OrderHistoryQueueLocation.Default;
        }
        #endregion

        #region methods
        public string ConsumeFromQueue() {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQServer,
                UserName = Configuration.RabbitMQUserNameConsumer,
                Password = Configuration.RabbitMQUserPasswordConsumer,
                VirtualHost = Configuration.RabbitMQVHostConfirmation
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {
                    BasicGetResult result = model.BasicGet(GetSelectedQueue(), true);

                    if (result == null) {
                        return null;
                    } else {
                        return Encoding.UTF8.GetString(result.Body);
                    }
                }
            }

        }

        /// <summary>
        /// Returns the selected exchange. This is future proofed to support the possibility of different exchanges.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedExchange() {
            return Configuration.RabbitMQExchangeConfirmation;
        }

        /// <summary>
        /// Returns the selected queue. This is setup to support the possibility of different queues in the future.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedQueue() {
            switch (_queuePath) {
                case OrderHistoryQueueLocation.Default:
                    return Configuration.RabbitMQQueueHourlyUpdates;
                default:
                    return Configuration.RabbitMQQueueHourlyUpdates;
            }
        }

        /// <summary>
        /// Publish data to RabbitMQ
        /// </summary>
        /// <param name="item"></param>
        public void PublishToQueue(string item) {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQServer,
                UserName = Configuration.RabbitMQUserNamePublisher,
                Password = Configuration.RabbitMQUserPasswordPublisher,
                VirtualHost = Configuration.RabbitMQVHostConfirmation
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {
                    string exchange = GetSelectedExchange();

                    model.QueueBind(GetSelectedQueue(), exchange, string.Empty, new Dictionary<string, object>());

                    IBasicProperties props = model.CreateBasicProperties();
                    props.DeliveryMode = 2; // persistent delivery mode

                    model.BasicPublish(exchange, string.Empty, false, props, Encoding.UTF8.GetBytes(item));
                }
            }
        }

        public void SetQueuePath(int pathEnum) {
            _queuePath = (OrderHistoryQueueLocation)pathEnum;
        }
        #endregion
    }
}
