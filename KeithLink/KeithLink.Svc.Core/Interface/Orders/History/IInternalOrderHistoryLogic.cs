using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Orders;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IInternalOrderHistoryLogic {
        Order GetOrder(string branchId, string invoiceNumber);

        List<Order> GetOrders(UserSelectedContext customerInfo);
    }
}
