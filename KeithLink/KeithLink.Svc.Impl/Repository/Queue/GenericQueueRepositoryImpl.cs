using KeithLink.Svc.Core.Exceptions.Queue;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KeithLink.Svc.Impl.Repository.Queue
{
    public class GenericQueueRepositoryImpl : IGenericQueueRepository
    {
        #region methods
        public string ConsumeFromQueue(string server, string username, string password, string virtualHost, string queue) {
            try {
                ConnectionFactory connectionFactory = new ConnectionFactory() {
                    HostName = server,
                    UserName = username,
                    Password = password,
                    VirtualHost = virtualHost
                };

                using (IConnection connection = connectionFactory.CreateConnection()) {
                    using (IModel model = connection.CreateModel()) {
                        BasicGetResult result = model.BasicGet(queue, true);

                        if (result == null) {
                            return null;
                        } else {
                            return Encoding.UTF8.GetString(result.Body);
                        }
                    }
                }
            } catch (Exception ex) {
                throw new QueueConnectionException(server, virtualHost, string.Empty, queue, ex.Message, ex);
            }
        }

        /// <summary>
        /// Publish data to RabbitMQ
        /// </summary>
        /// <param name="item"></param>
        public void PublishToQueue(string item, string server, string username, string password, string virtualHost, string exchange) {
            try {
                ConnectionFactory connectionFactory = new ConnectionFactory() {
                    HostName = server,
                    UserName = username,
                    Password = password,
                    VirtualHost = virtualHost
                };

                using (IConnection connection = connectionFactory.CreateConnection()) {
                    using (IModel model = connection.CreateModel()) {
                        IBasicProperties props = model.CreateBasicProperties();
                        props.DeliveryMode = 2; // persistent delivery mode

                        model.BasicPublish(exchange, string.Empty, false, props, Encoding.UTF8.GetBytes(item));
                    }
                }
            } catch (Exception ex) {
                throw new QueueConnectionException(server, virtualHost, exchange, string.Empty, ex.Message, ex);
            }
        }
        #endregion
    }
}
