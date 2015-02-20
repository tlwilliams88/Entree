using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents.Utility;
using CommerceServer.Core.Orders;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class SiteHelper
    {
        public static string GetSiteName()
        {
            return System.Configuration.ConfigurationManager.AppSettings["CS_Sitename"];
        }

        static CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection siteConfig = null;
        public static CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection GetCsConfig()
        {
            if (siteConfig == null)
            {
                siteConfig = new CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection(SiteHelper.GetSiteName());
            }
            return siteConfig;
        }

        static CommerceServer.Core.Runtime.Orders.OrderContext orderContext = null;
        public static CommerceServer.Core.Runtime.Orders.OrderContext GetOrderContext()
        {
            if (orderContext == null)
            {
                orderContext = CommerceServer.Core.Runtime.Orders.OrderContext.Create(GetSiteName());
            }
            return orderContext;
        }

		static CommerceServer.Core.Orders.OrderManagementContext orderMgmtContext = null;
		public static CommerceServer.Core.Orders.OrderManagementContext GetOrderManageContext()
		{
			if (orderMgmtContext == null)
			{
				var ordersAgent = new OrderSiteAgent(GetSiteName());
				orderMgmtContext = OrderManagementContext.Create(ordersAgent);
			}
			return orderMgmtContext;
		}

    }
}