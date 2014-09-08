using System;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderQueueRepository
    {
        void ConsumeOrders();

        void PublishOrder(Models.Orders.OrderFile order);
    }
}
