using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace KeithLink.Svc.Impl.Repository.Orders
{
    public class OrderQueueRepositoryImpl : IOrderQueueRepository
    {
        #region IOrderQueueRepository Members

        public void ConsumeOrders()
        {
            
        }

        public void PublishOrder(OrderFile order)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
