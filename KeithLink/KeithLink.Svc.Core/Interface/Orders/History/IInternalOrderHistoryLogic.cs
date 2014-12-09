using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Orders;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IInternalOrderHistoryLogic {
        List<Order> GetOrders(UserSelectedContext customerInfo);
    }
}
