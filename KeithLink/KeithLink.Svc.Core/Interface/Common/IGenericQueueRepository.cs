using KeithLink.Svc.Core.Models.Common;

namespace KeithLink.Svc.Core.Interface.Common {
    public interface IGenericQueueRepository {
        string ConsumeFromQueue(string serverName, string userName, string password, string virtualHost, string queue);

        void PublishToQueue(string item, string serverName, string userName, string password, string virtualHost, string exchange);
    }
}
