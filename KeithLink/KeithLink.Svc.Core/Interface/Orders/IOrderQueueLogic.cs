using KeithLink.Svc.Core.Models.Orders;
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
        void WriteFileToQueue(string orderingUserEmail, string orderNumber, CS.PurchaseOrder order, Enumerations.Order.OrderType orderType, string catalogType,
            string dsrNumber = "", string addressStreet = "", string addressCity = "", string addressState = "", string addressZip = "");
    }
}
