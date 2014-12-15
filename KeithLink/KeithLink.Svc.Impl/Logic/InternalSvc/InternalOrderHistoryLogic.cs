using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic.InternalSvc {
    public class InternalOrderHistoryLogic : IInternalOrderHistoryLogic {
        #region attributes
        private readonly IOrderHistoryLogic _histLogic;
        #endregion

        #region ctor
        public InternalOrderHistoryLogic(IOrderHistoryLogic orderHistoryLogic) {
            _histLogic = orderHistoryLogic;
        }
        #endregion

        #region methods
        public Order GetOrder(string branchId, string invoiceNumber) {
            return _histLogic.ReadOrder(branchId, invoiceNumber);
        }

        public List<Order> GetOrders(UserSelectedContext customerInfo) {
            return _histLogic.GetOrders(customerInfo);
        }
        #endregion
    }
}
