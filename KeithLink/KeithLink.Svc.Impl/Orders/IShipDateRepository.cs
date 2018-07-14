using Entree.Core.Models.Orders;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Orders {
    public interface IShipDateRepository {
        ShipDateReturn GetShipDates(UserSelectedContext customerInfo);
    }
}
