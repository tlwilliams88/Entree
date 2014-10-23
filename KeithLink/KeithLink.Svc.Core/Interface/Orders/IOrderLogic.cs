﻿using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderLogic
    {
		List<Order> ReadOrders(UserProfile userProfile, UserSelectedContext catalogInfo);
		Order ReadOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber);
        Order UpdateOrder(UserSelectedContext catalogInfo, UserProfile user, Order order, bool deleteOmmitedItems);
        NewOrderReturn SubmitChangeOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber);
    }
}
