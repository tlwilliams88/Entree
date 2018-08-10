using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.PowerMenu.Order;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.ShoppingCart;

namespace KeithLink.Svc.Core.Interface.Cart
{
    public interface IShoppingCartService
    {
        Guid ImportFromPowerMenu(VendorPurchaseOrderRequest po);
        ApprovedCartModel ValidateCartAmount(UserProfile user, UserSelectedContext catalogInfo, Guid cartId, String orderNumber);
    }
}
