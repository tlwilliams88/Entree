using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace KeithLink.Svc.InternalSvc.Interfaces
{
	[ServiceContract]
	public interface IOrderService
	{
		[OperationContract]
		DateTime? ReadLatestOrderModifiedDateForCustomer(UserSelectedContext catalogInfo);

        [OperationContract]
        List<Core.Models.Orders.History.OrderHistoryFile> GetLastFiveOrderHistory(UserSelectedContext catalogInfo, string itemNumber);

        [OperationContract]
        List<Order> GetCustomerOrders(Guid userId, UserSelectedContext catalogInfo);

        [OperationContract]
        Order GetOrder(string branchId, string invoiceNumber);

        [OperationContract]
        List<Order> GetOrderHeaderInDateRange(Guid userId, UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);
        
		[OperationContract]
		UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId);

		[OperationContract]
		void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId);

        [OperationContract]
        void SaveOrderHistory(OrderHistoryFile historyFile);

        [OperationContract]
        List<OrderHeader> GetSubmittedUnconfirmedOrders();

        [OperationContract]
        Guid GetUserIdForControlNumber(int controlNumber);

		[OperationContract]
		PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, PagingModel paging);

	}
}