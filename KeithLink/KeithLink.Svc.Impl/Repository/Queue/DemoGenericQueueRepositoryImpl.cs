using KeithLink.Svc.Core.Exceptions.Queue;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Orders;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Queue
{
	public class DemoGenericQueueRepositoryImpl : IGenericQueueRepository
	{
		//private static DemoRabbitMQ demoRabbitMQ = new DemoRabbitMQ();

		public string ConsumeFromQueue(string server, string username, string password, string virtualHost, string queue)
		{
			return null;
			//Reading from the queue will behave the same in the demo environment as normal
			//try
			//{
			//	ConnectionFactory connectionFactory = new ConnectionFactory()
			//	{
			//		HostName = server,
			//		UserName = username,
			//		Password = password,
			//		VirtualHost = virtualHost
			//	};

			//	using (IConnection connection = connectionFactory.CreateConnection())
			//	{
			//		using (IModel model = connection.CreateModel())
			//		{
			//			BasicGetResult result = model.BasicGet(queue, true);

			//			if (result == null)
			//			{
			//				return null;
			//			}
			//			else
			//			{
			//				return Encoding.UTF8.GetString(result.Body);
			//			}
			//		}
			//	}
			//}
			//catch (Exception ex)
			//{
			//	throw new QueueConnectionException(server, virtualHost, string.Empty, queue, ex.Message, ex);
			//}
		}

		public void PublishToQueue(string item, string server, string username, string password, string virtualHost, string exchange)
		{


			//Do nothing



			//For the demo environment, nothing is being sent to the mainframe. So for messages that would trigger a communication to the mainframe will be changed to just mimic the response from the MF

			//if (exchange == Configuration.RabbitMQExchangeOrdersCreated)//Order creation. Generate an order confirmation message and place it on the queue
			//{
			//	//Send to order history queue
			//	PublishFinalMessageToQueue(item, Configuration.RabbitMQOrderServer, Configuration.RabbitMQUserNamePublisher, Configuration.RabbitMQUserPasswordPublisher, Configuration.RabbitMQVHostOrder, Configuration.RabbitMQExchangeOrdersHistory);

			//	//Generate a fake confirmation
			//	GenerateAndPublishOrderConfirmation(JsonConvert.DeserializeObject<OrderFile>(item));
			//}
			//else
			//	PublishFinalMessageToQueue(item, server, username, password, virtualHost, exchange);
			
			
		}

		private void PublishFinalMessageToQueue(string item, string server, string username, string password, string virtualHost, string exchange)
		{
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
					connection.AutoClose = false; //to prevent using block from attempting to close a connection that DNE

					using (IModel model = connection.CreateModel())
					{
						IBasicProperties props = model.CreateBasicProperties();
						props.DeliveryMode = 2; // persistent delivery mode

						model.BasicPublish(exchange, string.Empty, false, props, Encoding.UTF8.GetBytes(item));
					}
				}
			}
			catch (Exception ex)
			{
				throw new QueueConnectionException(server, virtualHost, exchange, string.Empty, ex.Message, ex);
			}
		}

		private void GenerateAndPublishOrderConfirmation(OrderFile order)
		{

		}



		public void BulkPublishToQueue(List<string> items, string server, string username, string password, string virtualHost, string exchange)
		{
			
		}
	}


	//public class DemoRabbitMQ
	//{
		
	//	public Dictionary<string, Queue<string>> demoQueue = new Dictionary<string,Queue<string>>();

	//	public string Dequeue(string queue)
	//	{
	//		if (demoQueue.ContainsKey(queue))
	//		{
	//			if(demoQueue[queue].Count > 0)
	//				return demoQueue[queue].Dequeue();
	//		}

	//		return null;
	//	}

	//	public void Enqueue(string item, string queue)
	//	{
	//		if (!demoQueue.ContainsKey(queue))
	//			demoQueue.Add(queue, new Queue<string>());

	//		demoQueue[queue].Enqueue(item);

	//	}


	//}
}
