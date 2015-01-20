using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Exceptions.Queue;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Common;
using KeithLink.Svc.Core.Models.Orders;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Orders {
	//public class OrderQueueRepositoryImpl : IOrderQueueRepository {
	//	#region attributes
	//	private OrderQueueLocation _queuePath;
	//	#endregion
        
	//	#region ctor
	//	public OrderQueueRepositoryImpl() {
	//		_queuePath = OrderQueueLocation.Normal;
	//	}
	//	#endregion

	//	#region methods
	//	public string ConsumeFromQueue() {
	//		try {
	//			ConnectionFactory connectionFactory = new ConnectionFactory() {
	//				HostName = Configuration.RabbitMQOrderServer,
	//				UserName = Configuration.RabbitMQUserNameConsumer,
	//				Password = Configuration.RabbitMQUserPasswordConsumer,
	//				VirtualHost = Configuration.RabbitMQVHostOrder
	//			};

	//			using (IConnection connection = connectionFactory.CreateConnection()) {
	//				using (IModel model = connection.CreateModel()) {
	//					BasicGetResult result = model.BasicGet(GetSelectedQueue(), true);

	//					if (result == null) {
	//						return null;
	//					} else {
	//						return Encoding.UTF8.GetString(result.Body);
	//					}
	//				}
	//			}
	//		} catch (Exception ex) {
	//			throw new QueueConnectionException(Configuration.RabbitMQOrderServer, Configuration.RabbitMQVHostOrder, string.Empty, GetSelectedQueue(), ex.Message, ex);
	//		}
	//	}

	//	private string GetSelectedExchange() {
	//		switch (_queuePath) {
	//			case OrderQueueLocation.Normal:
	//				return Configuration.RabbitMQExchangeOrdersCreated;
	//			case OrderQueueLocation.History:
	//				return Configuration.RabbitMQExchangeOrdersHistory;
	//			case OrderQueueLocation.Error:
	//				return Configuration.RabbitMQExchangeOrdersError;
	//			default:
	//				return Configuration.RabbitMQExchangeOrdersCreated;
	//		}
	//	}

	//	private string GetSelectedQueue() {
	//		switch (_queuePath) {
	//			case OrderQueueLocation.Normal:
	//				return Configuration.RabbitMQQueueOrderCreated;
	//			case OrderQueueLocation.History:
	//				return Configuration.RabbitMQQueueOrderHistory;
	//			case OrderQueueLocation.Error:
	//				return Configuration.RabbitMQQueueOrderError;
	//			default:
	//				return Configuration.RabbitMQQueueOrderCreated;
	//		}
	//	}

	//	public void PublishToQueue(string item) {
	//		try {
	//			// this connection uses different credentials than the other methods in this class
	//			ConnectionFactory connectionFactory = new ConnectionFactory() {
	//				HostName = Configuration.RabbitMQOrderServer,
	//				UserName = Configuration.RabbitMQUserNamePublisher,
	//				Password = Configuration.RabbitMQUserPasswordPublisher,
	//				VirtualHost = Configuration.RabbitMQVHostOrder
	//			};

	//			using (IConnection connection = connectionFactory.CreateConnection()) {
	//				using (IModel model = connection.CreateModel()) {
	//					string exchange = GetSelectedExchange();

	//					model.QueueBind(GetSelectedQueue(), exchange, string.Empty, new Dictionary<string, object>());

	//					IBasicProperties props = model.CreateBasicProperties();
	//					props.DeliveryMode = 2; // persistent delivery mode

	//					model.BasicPublish(exchange, string.Empty, false, props, Encoding.UTF8.GetBytes(item));
	//				}
	//			}
	//		} catch (Exception ex) {
	//			throw new QueueConnectionException(Configuration.RabbitMQOrderServer, Configuration.RabbitMQVHostOrder, GetSelectedExchange(), GetSelectedQueue(), ex.Message, ex);                
	//		}
	//	}

	//	public void SetQueuePath(int enumPath) {
	//		_queuePath = (OrderQueueLocation) enumPath;
	//	}
	//	#endregion

	//}
}
