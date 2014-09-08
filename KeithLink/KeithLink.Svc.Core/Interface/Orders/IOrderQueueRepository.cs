using System;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderQueueRepository
    {
        void ConsumeOrders();

        void PublishOrder(string rawOrderFile);
    }
}
