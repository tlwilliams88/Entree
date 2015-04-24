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
		}

		public void PublishToQueue(string item, string server, string username, string password, string virtualHost, string exchange)
		{
			//Do nothing
		}
		
		public void BulkPublishToQueue(List<string> items, string server, string username, string password, string virtualHost, string exchange)
		{
			
		}
	}

}
