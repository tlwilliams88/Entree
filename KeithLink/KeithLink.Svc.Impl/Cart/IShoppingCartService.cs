using Entree.Core.Models.Profile;
using Entree.Core.Models.PowerMenu.Order;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Cart
{
    public interface IShoppingCartService
    {
        Guid ImportFromPowerMenu(VendorPurchaseOrderRequest po);
    }
}
