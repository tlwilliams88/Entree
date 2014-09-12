using KeithLink.Svc.Core.Models.Common;

namespace KeithLink.Svc.Core.Interface.Common {
    public interface IQueueRepository {
        string ConsumeFromQueue();

        void PublishToQueue(string item);

        void SetQueuePath(int pathEnum);
    }
}
