using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Orders
{
    public class OrderQueueRepositoryImpl : IOrderQueueRepository {

        #region attributes
        private ISocketConnectionRepository _mfCon;
        private IEventLogRepository _log;
        #endregion

        #region ctor
        public OrderQueueRepositoryImpl(IEventLogRepository EventLog, ISocketConnectionRepository MainframeConnection) {
            _log = EventLog;
            _mfCon = MainframeConnection;
        }
        #endregion

        #region methods
        public void ConsumeOrders() {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQOrderServer,
                UserName = Configuration.RabbitMQOrderConsumerUserName,
                Password = Configuration.RabbitMQOrderConsumerUserPassword,
                VirtualHost = Configuration.RabbitMQOrderVHost
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {
                    QueueingBasicConsumer consumer = new QueueingBasicConsumer(model);
                    model.BasicConsume(Configuration.RabbitMQOrderQueue, false, Configuration.ApplicationName, consumer);

                    while (true) {
                        try {
                            BasicDeliverEventArgs eventArgs = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                            // order processing logic

                            model.BasicAck(eventArgs.DeliveryTag, false);
                        } catch (OperationInterruptedException operationEx) {
                            StringBuilder msg = new StringBuilder();
                            msg.AppendLine("Order Consumtion Interrupted!");
                            msg.AppendLine(string.Concat("Message: ", operationEx.Message));
                            msg.AppendLine(string.Concat("Shutdown Reason:", operationEx.ShutdownReason));
                            msg.AppendLine("Data:");

                            foreach(string key in operationEx.Data.Keys){
                                msg.Append("/t");
                                msg.Append(key);
                                msg.Append(" = ");
                                msg.AppendLine(operationEx.Data[key].ToString());
                            }
                            
                            _log.WriteErrorLog(msg.ToString(), operationEx);
                        }
                    }
                }
            }
        }

        public void PublishOrder(OrderFile order) {
            ConnectionFactory connectionFactory = new ConnectionFactory() {
                HostName = Configuration.RabbitMQOrderServer,
                UserName = Configuration.RabbitMQOrderPublisherUserName,
                Password = Configuration.RabbitMQOrderPublisherUserPassword,
                VirtualHost = Configuration.RabbitMQOrderVHost
            };

            using (IConnection connection = connectionFactory.CreateConnection()) {
                using (IModel model = connection.CreateModel()) {
                    model.ExchangeDeclare(Configuration.RabbitMQExchangeName, "fanout", true);
                    model.QueueDeclare(Configuration.RabbitMQOrderQueue, true, false, false, new Dictionary<string, object>());
                    model.QueueBind(Configuration.RabbitMQOrderQueue, Configuration.RabbitMQExchangeName, string.Empty, new Dictionary<string, object>());

                    IBasicProperties props = model.CreateBasicProperties();
                    props.DeliveryMode = 2; // persistent delivery mode

                    model.BasicPublish(Configuration.RabbitMQExchangeName, string.Empty, false, props, Encoding.UTF8.GetBytes(order.ToString()));
                }
            }
        }

        #endregion
    }
}
