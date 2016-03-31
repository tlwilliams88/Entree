using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;


namespace KeithLink.Svc.Core.Interface.Orders
{
    public interface IOrderLogic
    {
        NewOrderReturn CancelOrder(UserProfile userProfile, UserSelectedContext catalogInfo, Guid commerceId);

        Order GetOrder(string branchId, string invoiceNumber);

        List<Order> GetOrderHeaderInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);

        List<Order> GetOrders(System.Guid userId, UserSelectedContext customerInfo);

        List<OrderHeader> GetSubmittedUnconfirmedOrders();

        PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, PagingModel paging);

        Order ReadOrder( UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber, bool omitDeletedItems = true );

        List<Order> ReadOrderHistories( UserProfile userProfile, UserSelectedContext catalogInfo, bool omitDeletedItems = true );

        List<Order> ReadOrders( UserProfile userProfile, UserSelectedContext catalogInfo, bool omitDeletedItems = true, bool header = false );

        bool ResendUnconfirmedOrder( UserProfile userProfile, int controlNumber, UserSelectedContext catalogInfo );

        NewOrderReturn SubmitChangeOrder( UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber );

        Order UpdateOrder( UserSelectedContext catalogInfo, UserProfile user, Order order, bool deleteOmmitedItems );

        Order UpdateOrderForEta( UserProfile user, Order order );

        List<Order> UpdateOrdersForSecurity(UserProfile user, List<Order> orders);
    }
}
