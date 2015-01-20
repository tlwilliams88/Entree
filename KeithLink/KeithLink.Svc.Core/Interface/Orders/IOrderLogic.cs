using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderLogic
    {
		List<Order> ReadOrders(UserProfile userProfile, UserSelectedContext catalogInfo, bool omitDeletedItems = true);
        List<Order> ReadOrderHistories(UserProfile userProfile, UserSelectedContext catalogInfo, bool omitDeletedItems = true);
		Order ReadOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber, bool omitDeletedItems = true);
        Order UpdateOrder(UserSelectedContext catalogInfo, UserProfile user, Order order, bool deleteOmmitedItems);
        NewOrderReturn SubmitChangeOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber);
        NewOrderReturn CancelOrder(UserProfile userProfile, UserSelectedContext catalogInfo, Guid commerceId);
        Order UpdateOrderForEta(UserProfile user, Order order);
        List<Order> UpdateOrdersForSecurity(UserProfile user, List<Order> orders);
    }
}
