using KeithLink.Svc.Core.Models.Common;
using RabbitMQ.Client;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Common {
    public interface IGenericQueueRepository {
        string ConsumeFromQueue(string serverName, string userName, string password, string virtualHost, string queue);
        void PublishToQueue(string item, string serverName, string userName, string password, string virtualHost, string exchange);
        void PublishToDirectedExchange(string item, string serverName, string userName, string password, string virtualHost, string exchange, string routeKey);
        void BulkPublishToQueue(List<string> items, string server, string username, string password, string virtualHost, string exchange);
        QueueDeclareOk PassivelyDeclareQueue
            (string server, string username, string password, string virtualHost, string queue);
    }
}
