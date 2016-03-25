using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IOrderHistoryLogic {
        string CheckForLostOrders(out string sBody);

        Order GetOrder(string branchId, string invoiceNumber);

        List<Order> GetOrderHeaderInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);

        List<Order> GetOrders(System.Guid userId, UserSelectedContext customerInfo);

        OrderTotalByMonth GetOrderTotalByMonth(UserSelectedContext customerInfo, int numberOfMonths);

        PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, PagingModel paging);

        void ListenForMainFrameCalls();

        void ListenForQueueMessages();

        OrderHistoryFileReturn ParseMainframeFile(string rawFlatFile);

        void SaveOrder(OrderHistoryFile historyFile, bool isSpecialOrder);

        string SetLostOrder(string trackingNumber);

        void StopListening();

        void UpdateRelatedOrderNumber(string childOrderNumber, string parentOrderNumber);
    }
}
