using KeithLink.Svc.Core.Models.Common;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Common {
    public interface IGenericQueueRepository {
        string ConsumeFromQueue(string serverName, string userName, string password, string virtualHost, string queue);

        void PublishToQueue(string item, string serverName, string userName, string password, string virtualHost, string exchange);
		void BulkPublishToQueue(List<string> items, string server, string username, string password, string virtualHost, string exchange);
    }
}
