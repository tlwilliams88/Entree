using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Invoices;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Core.Models.ShoppingCart;

namespace KeithLink.Svc.Impl.Helpers
{
    public class ItemOrderHistoryHelper
    {
        public static void GetItemOrderHistories(ICatalogLogic catalogLogic, UserSelectedContext catalogInfo, List<ItemUsageReportItemModel> prods) {
            foreach (var prod in prods) {
                StringBuilder sbHistory = BuildItemHistory(catalogLogic, catalogInfo, prod.ItemNumber);
                prod.OrderHistoryString = sbHistory.ToString();
            }
        }

        public static void GetItemOrderHistories(ICatalogLogic catalogLogic, UserSelectedContext catalogInfo, List<OrderLine> prods)
        {
            foreach (var prod in prods) {
                StringBuilder sbHistory = BuildItemHistory(catalogLogic, catalogInfo, prod.ItemNumber);
                prod.OrderHistoryString = sbHistory.ToString();
            }
        }

        public static void GetItemOrderHistories(ICatalogLogic catalogLogic, UserSelectedContext catalogInfo, List<ShoppingCartItem> prods)
        {
            foreach (var prod in prods)
            {
                StringBuilder sbHistory = BuildItemHistory(catalogLogic, catalogInfo, prod.ItemNumber);
                prod.OrderHistoryString = sbHistory.ToString();
            }
        }

        public static void GetItemOrderHistories(ICatalogLogic catalogLogic, UserSelectedContext catalogInfo, List<Product> prods)
        {
            foreach (var prod in prods) {
                StringBuilder sbHistory = BuildItemHistory(catalogLogic, catalogInfo, prod.ItemNumber);
                prod.OrderHistoryString = sbHistory.ToString();
            }
        }

        public static void GetItemOrderHistories(ICatalogLogic catalogLogic, UserSelectedContext catalogInfo, List<ListItemModel> prods)
        {
            foreach (var prod in prods)
            {
                StringBuilder sbHistory = BuildItemHistory(catalogLogic, catalogInfo, prod.ItemNumber);
                prod.OrderHistoryString = sbHistory.ToString();
            }
        }

        public static void GetItemOrderHistories(ICatalogLogic catalogLogic, UserSelectedContext catalogInfo, List<InvoiceItemModel> prods)
        {
            foreach (var prod in prods)
            {
                StringBuilder sbHistory = BuildItemHistory(catalogLogic, catalogInfo, prod.ItemNumber);
                prod.OrderHistoryString = sbHistory.ToString();
            }
        }

        private static StringBuilder BuildItemHistory(ICatalogLogic catalogLogic, UserSelectedContext catalogInfo, string itemNumber)
        {
            var history = catalogLogic.GetLastFiveOrderHistory(catalogInfo, itemNumber);
            StringBuilder sbHistory = new StringBuilder();
            if (history != null) {
                foreach (OrderHistoryFile h in history)
                {
                    foreach (OrderHistoryDetail d in h.Details)
                    {
                        sbHistory.Append(string.Format("{0}-{1}, ", h.Header.DeliveryDate, d.ShippedQuantity));
                    }
                }
            }
            return sbHistory;
        }

    }
}
