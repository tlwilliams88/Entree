using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Orders.History;
using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KeithLink.Svc.Impl.Repository.Orders.History {
    public class OrderUpdateRequestQueueRepositoryImpl : IQueueRepository {
        #region methods
        public string ConsumeFromQueue() {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQConfirmationServer,
                UserName = Configuration.RabbitMQUserNameConsumer,
                Password = Configuration.RabbitMQUserPasswordConsumer,
                VirtualHost = Configuration.RabbitMQVHostConfirmation
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {
                    BasicGetResult result = model.BasicGet(Configuration.RabbitMQQueueOrderUpdateRequest, true);

                    if (result == null) {
                        return null;
                    } else {
                        return Encoding.UTF8.GetString(result.Body);
                    }
                }
            }
        }

        /// <summary>
        /// Publish data to RabbitMQ
        /// </summary>
        /// <param name="item"></param>
        public void PublishToQueue(string item) {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQConfirmationServer,
                UserName = Configuration.RabbitMQUserNamePublisher,
                Password = Configuration.RabbitMQUserPasswordPublisher,
                VirtualHost = Configuration.RabbitMQVHostConfirmation
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {

                    model.QueueBind(Configuration.RabbitMQQueueOrderUpdateRequest, Configuration.RabbitMQExchangeOrderUpdateRequests, string.Empty, new Dictionary<string, object>());

                    IBasicProperties props = model.CreateBasicProperties();
                    props.DeliveryMode = 2; // persistent delivery mode

                    model.BasicPublish(Configuration.RabbitMQExchangeOrderUpdateRequests, string.Empty, false, props, Encoding.UTF8.GetBytes(item));
                }
            }
        }
        #endregion
    }
}
