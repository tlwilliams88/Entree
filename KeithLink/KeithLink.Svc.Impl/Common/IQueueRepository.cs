using Entree.Core.Models.Common;

namespace Entree.Core.Interface.Common {
    public interface IQueueRepository {
        string ConsumeFromQueue();

        void PublishToQueue(string item);
    }
}
