using KeithLink.Svc.Core.Models.Orders;

namespace KeithLink.Svc.Core.Interface.Orders {
    public interface IShipDateRepository {
        ShipDateReturn GetShipDates(string branchId, string customerNumber);
    }
}
