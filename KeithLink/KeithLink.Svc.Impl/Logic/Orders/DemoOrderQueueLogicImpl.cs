using KeithLink.Svc.Core.Interface.Orders;
using System;
using System.Text;

namespace KeithLink.Svc.Impl.Logic.Orders {
    public class DemoOrderQueueLogicImpl : IOrderQueueLogic {
        #region methods
        public void ProcessOrders() {
        }

        public void WriteFileToQueue(string orderingUserEmail, string orderNumber, Core.Models.Generated.PurchaseOrder order, Core.Enumerations.Order.OrderType orderType) {
        }
        #endregion
    }
}
