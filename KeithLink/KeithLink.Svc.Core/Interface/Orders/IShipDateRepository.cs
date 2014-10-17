using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Orders {
    public interface IShipDateRepository {
        ShipDateReturn GetShipDates(UserSelectedContext customerInfo);
    }
}
