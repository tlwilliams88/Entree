using CommerceServer.Core.Orders;
using CommerceServer.Core.Runtime.Configuration;
using CommerceServer.Core.Runtime.Orders;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents.Utility;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Extensions {
    public class SiteHelper {
        #region attributes
        static CommerceResourceCollection siteConfig = null;
        static OrderContext orderContext = null;
        static OrderManagementContext orderMgmtContext = null;
        #endregion

        #region ctor
        #endregion

        #region methods
        public static CommerceResourceCollection GetCsConfig() {
            if (siteConfig == null) {
                siteConfig = new CommerceResourceCollection(SiteHelper.GetSiteName());
            }

            return siteConfig;
        }

        public static OrderContext GetOrderContext() {
            if (orderContext == null) {
                orderContext = OrderContext.Create(GetSiteName());
            }

            return orderContext;
        }

        public static OrderManagementContext GetOrderManageContext() {
            if (orderMgmtContext == null) {
                var ordersAgent = new OrderSiteAgent(GetSiteName());
                orderMgmtContext = OrderManagementContext.Create(ordersAgent);
            }

            return orderMgmtContext;
        }

        public static string GetSiteName() {
            return ConfigurationManager.AppSettings["CS_Sitename"];
        }
        #endregion

        #region properties
        #endregion
    }
}