using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Orders;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Orders {
    public class OrderQueueRepositoryImpl : IQueueRepository {
        #region attributes
        private OrderQueueLocation _queuePath;
        #endregion
        
        #region ctor
        public OrderQueueRepositoryImpl() {
            _queuePath = OrderQueueLocation.Normal;
        }
        #endregion

        #region methods
        public string ConsumeFromQueue() {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQOrderServer,
                UserName = Configuration.RabbitMQOrderConsumerUserName,
                Password = Configuration.RabbitMQOrderConsumerUserPassword,
                VirtualHost = Configuration.RabbitMQOrderVHost
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

        private string GetSelectedExchange() {
            switch (_queuePath) {
                case OrderQueueLocation.Normal:
                    return Configuration.RabbitMQExchangeOrdersCreated;
                case OrderQueueLocation.History:
                    return Configuration.RabbitMQExchangeOrdersHistory;
                case OrderQueueLocation.Error:
                    return Configuration.RabbitMQExchangeOrdersError;
                default:
                    return Configuration.RabbitMQExchangeOrdersCreated;
            }
        }

        private string GetSelectedQueue() {
            switch (_queuePath) {
                case OrderQueueLocation.Normal:
                    return Configuration.RabbitMQOrderCreatedQueue;
                case OrderQueueLocation.History:
                    return Configuration.RabbitMQOrderHistoryQueue;
                case OrderQueueLocation.Error:
                    return Configuration.RabbitMQOrderErrorQueue;
                default:
                    return Configuration.RabbitMQOrderCreatedQueue;
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
                    string exchange = GetSelectedExchange();

                    model.QueueBind(GetSelectedQueue(), exchange, string.Empty, new Dictionary<string, object>());

                    IBasicProperties props = model.CreateBasicProperties();
                    props.DeliveryMode = 2; // persistent delivery mode

                    model.BasicPublish(exchange, string.Empty, false, props, Encoding.UTF8.GetBytes(item));
                }
            }
        }

        public void SetQueuePath(int enumPath) {
            _queuePath = (OrderQueueLocation) enumPath;
        }
        #endregion

    }
}
