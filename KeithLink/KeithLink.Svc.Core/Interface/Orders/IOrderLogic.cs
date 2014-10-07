using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderLogic
    {
		List<Order> ReadOrders(UserProfile userProfile, CatalogInfo catalogInfo);
		Order ReadOrder(UserProfile userProfile, CatalogInfo catalogInfo, string orderNumber);
    }
}
