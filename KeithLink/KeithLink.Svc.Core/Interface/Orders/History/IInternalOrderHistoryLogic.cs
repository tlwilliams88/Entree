using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IInternalOrderHistoryLogic {
        Order GetOrder(string branchId, string invoiceNumber);

        List<Order> GetOrders(System.Guid userId, UserSelectedContext customerInfo);

		List<Order> GetOrderHeaderInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);

		PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, PagingModel paging);
        OrderTotalByMonth GetOrderTotalByMonth( UserSelectedContext customerInfo, int numberOfMonths );
        void SaveOrder(OrderHistoryFile historyFile, bool isSpecialOrder);

        void UpdateRelatedOrderNumber(string childOrderNumber, string parentOrderNumber);

		void ListenForQueueMessages();

        void StopListening();

        string CheckForLostOrders(out string sBody);

        string SetLostOrder(string trackingNumber);
    }
}
