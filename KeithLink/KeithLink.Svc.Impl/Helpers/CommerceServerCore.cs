using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceServer.Core.Runtime.Orders;
using CommerceServer.Core.Orders;
using CommerceServer.Core;

namespace KeithLink.Svc.Impl.Helpers
{
    public class CommerceServerCore
    {
        static OrderContext orderContext = null;
        static OrderSiteAgent ordersAgent = null;
        static OrderManagementContext context = null;
        static PurchaseOrderManager manager = null;

        private static void LoadOrderContext()
        {
            if (orderContext == null)
            {
                var siteName = Configuration.CSSiteName;
                ordersAgent = new OrderSiteAgent(siteName);
                context = OrderManagementContext.Create(ordersAgent);
                manager = context.PurchaseOrderManager;
                orderContext = CommerceServer.Core.Runtime.Orders.OrderContext.Create(siteName);
            }
        }

        public static PurchaseOrderManager GetPoManager()
        {
            if (manager == null)
                LoadOrderContext();
            return manager;
        }

        public static OrderContext GetOrderContext()
        {
            if (context == null)
                LoadOrderContext();
            return orderContext;
        }
    }
}
