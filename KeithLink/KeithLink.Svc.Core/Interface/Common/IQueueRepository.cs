namespace KeithLink.Svc.Core.Interface.Common {
    public interface IQueueRepository {
        string ConsumeFromQueue();

        void PublishToQueue(string item);
    }
}
