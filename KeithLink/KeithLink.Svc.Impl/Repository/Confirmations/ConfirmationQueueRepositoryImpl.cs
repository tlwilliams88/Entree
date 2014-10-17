using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Confirmations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace KeithLink.Svc.Impl.Repository.Confirmations
{
    public class ConfirmationQueueRepositoryImpl : IQueueRepository
    {
        #region attributes

        private ConfirmationQueueLocation _queuePath;

        #endregion

        #region constructor

        public ConfirmationQueueRepositoryImpl()
        {
            _queuePath = ConfirmationQueueLocation.Default;
        }

        #endregion

        #region methods

        public string ConsumeFromQueue()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = Configuration.RabbitMQOrderServer,
                UserName = Configuration.RabbitMQOrderConsumerUserName,
                Password = Configuration.RabbitMQOrderConsumerUserPassword,
                VirtualHost = Configuration.RabbitMQConfirmationVhost
            };

            using (IConnection connection = connectionFactory.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    BasicGetResult result = model.BasicGet(GetSelectedQueue(), true);

                    if (result == null)
                    {
                        return null;
                    }
                    else
                    {
                        return Encoding.UTF8.GetString(result.Body);
                    }
                }
            }

        }

        /// <summary>
        /// Returns the selected exchange. This is future proofed to support the possibility of different exchanges.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedExchange()
        {
            return Configuration.RabbitMQConfirmationExchange;
        }

        /// <summary>
        /// Returns the selected queue. This is setup to support the possibility of different queues in the future.
        /// </summary>
        /// <returns></returns>
        private string GetSelectedQueue()
        {
            switch (_queuePath)
            {
                case ConfirmationQueueLocation.Default:
                    return Configuration.RabbitMQConfirmationQueue;
                default:
                    return Configuration.RabbitMQConfirmationQueue;
            }
        }

        /// <summary>
        /// Publish data to RabbitMQ
        /// </summary>
        /// <param name="item"></param>
        public void PublishToQueue(string item)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = Configuration.RabbitMQOrderServer,
                UserName = Configuration.RabbitMQOrderPublisherUserName,
                Password = Configuration.RabbitMQOrderPublisherUserPassword,
                VirtualHost = Configuration.RabbitMQConfirmationVhost
            };

            using (IConnection connection = connectionFactory.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    string exchange = GetSelectedExchange();

                    model.QueueBind(GetSelectedQueue(), exchange, string.Empty, new Dictionary<string, object>());

                    IBasicProperties props = model.CreateBasicProperties();
                    props.DeliveryMode = 2; // persistent delivery mode

                    model.BasicPublish(exchange, string.Empty, false, props, Encoding.UTF8.GetBytes(item));
                }
            }
        }

        /// <summary>
        /// Set the queue to publish to
        /// </summary>
        /// <param name="enumPath"></param>
        public void SetQueuePath(int enumPath)
        {
            _queuePath = (ConfirmationQueueLocation)enumPath;
        }
        #endregion

    }
}
