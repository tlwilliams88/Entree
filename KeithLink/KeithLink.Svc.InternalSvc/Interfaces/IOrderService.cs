using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
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
		UserActiveCartModel GetUserActiveCart(Guid userId);

		[OperationContract]
		void SaveUserActiveCart(Guid userId, Guid cartId);

		[OperationContract]
		List<Order> GetOrderHeaderInDateRange(Guid userId, UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);
	}
}