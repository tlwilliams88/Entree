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
    public class QueueManager
    {
        #region methods
        public List<QueueDeclareOk> AddTtlToQueue(string server, string username, string password, string virtualHost, List<(string name, int timeToLive)> queues)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = server,
                UserName = username,
                Password = password,
                VirtualHost = virtualHost
            };

            using (IConnection connection = connectionFactory.CreateConnection())
            {
                using (IModel model = connection.CreateModel())
                {
                    var responses = new List<QueueDeclareOk>();

                    foreach (var queue in queues)
                    {
                        var args = new Dictionary<string, object>();
                        args.Add("x-message-ttl", queue.timeToLive);    // time to live

                        QueueDeclareOk response = model.QueueDeclare(queue.name, true, false, false, args);
                        responses.Add(response);
                    }

                    return responses;
                }
            }
        }

        public QueueDeclareOk PassivelyDeclareQueue
            (string server, string username, string password, string virtualHost, string queue)
        {
            QueueDeclareOk result = null;
            try
            {
                ConnectionFactory connectionFactory = new ConnectionFactory()
                {
                    HostName = server,
                    UserName = username,
                    Password = password,
                    VirtualHost = virtualHost
                };

                using (IConnection connection = connectionFactory.CreateConnection())
                {
                    using (IModel model = connection.CreateModel())
                    {
                        result = model.QueueDeclarePassive(queue);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new QueueConnectionException(server, virtualHost, string.Empty, queue, ex.Message, ex);
            }
            return result;
        }

        #endregion
    }
}
