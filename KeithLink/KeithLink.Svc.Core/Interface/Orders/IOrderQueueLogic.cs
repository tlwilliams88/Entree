using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using CS = KeithLink.Svc.Core.Models.Generated;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderQueueLogic
    {
        void ProcessOrders();
        void SendToHost(OrderFile order);
        void WriteFileToQueue(OrderFile orderFile);
    }
}
