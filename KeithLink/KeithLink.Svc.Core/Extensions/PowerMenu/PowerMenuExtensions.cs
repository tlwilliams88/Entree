using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Extensions;

using KeithLink.Svc.Core.Models.PowerMenu.Order;
using KeithLink.Svc.Core.Models.ShoppingCart;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.PowerMenu
{
    public static class PowerMenuExtensions
    {
        public static ShoppingCartItem ToShoppingCartItem(this OrderItem orderItem, string catalogId )
        {
            ShoppingCartItem returnValue = new ShoppingCartItem();

            returnValue.ItemNumber = orderItem.VendorProductNumber;
            returnValue.Position = orderItem.CustomerLineSequenceNumber;
            returnValue.CatalogId = catalogId;

            if (orderItem.EachQuantityOrdered > 0)
            {
                returnValue.Each = true;
                returnValue.Quantity = orderItem.EachQuantityOrdered;
            } else
            {
                returnValue.Each = false;
                returnValue.Quantity = orderItem.CaseQuantityOrdered;
            }

            return returnValue;
        }

        public static List<ShoppingCartItem> ToShoppingCartItems(this List<OrderItem> orderItems, string catalogId)
        {
            List<ShoppingCartItem> returnValue = new List<ShoppingCartItem>();

            foreach (OrderItem item in orderItems)
            {
                returnValue.Add(item.ToShoppingCartItem(catalogId));
            }

            return returnValue;
        }
    }
}
