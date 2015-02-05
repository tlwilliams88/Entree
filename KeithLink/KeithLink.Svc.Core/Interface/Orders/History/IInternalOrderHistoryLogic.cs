using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Orders;
using System.Collections.Generic;
using System;
using KeithLink.Svc.Core.Models.Paging;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IInternalOrderHistoryLogic {
        Order GetOrder(string branchId, string invoiceNumber);

        List<Order> GetOrders(System.Guid userId, UserSelectedContext customerInfo);

		List<Order> GetOrderHeaderInDateRange(Guid userId, UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);

        void SaveOrder(OrderHistoryFile historyFile);

		PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, PagingModel paging);

		void ListenForQueueMessages();

		void StopListening();
    }
}
