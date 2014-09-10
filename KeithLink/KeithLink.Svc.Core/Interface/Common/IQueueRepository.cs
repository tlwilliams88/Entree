using KeithLink.Svc.Core.Models.Common;

namespace KeithLink.Svc.Core.Interface.Common {
    public interface IQueueRepository {
        void AcknowledgeReceipt(ulong DeliveryTag);

        QueueReturn ConsumeFromQueue();

        void DenyReceipt(ulong DeliveryTag);

        void PublishToQueue(string item);
    }
}
