using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Orders.History {
    public interface IInternalOrderHistoryLogic {
        Order GetOrder(string branchId, string invoiceNumber);

		List<Order> GetOrderHeaderInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);

        List<Order> GetOrders(System.Guid userId, UserSelectedContext customerInfo);

		PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, PagingModel paging);

		void ListenForQueueMessages();

        void SaveOrder(OrderHistoryFile historyFile);

		void StopListening();
    }
}
