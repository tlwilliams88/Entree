using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders
{
	public interface IOrderServiceRepository
	{
		DateTime? ReadLatestUpdatedDate(UserSelectedContext catalogInfo);

        List<OrderHistoryFile> GetLastFiveOrderHistory( UserSelectedContext catalogInfo, string itemNumber );

        List<Order> GetCustomerOrders(Guid userId, UserSelectedContext catalogInfo);

        Order GetOrder(string branchId, string invoiceNumber);

        List<Order> GetOrderHeaderInDateRange(Guid userId, UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);
        
		UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId);
		
        void SaveOrderHistory(OrderHistoryFile historyFile);
		
        void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId);

        // functions called by admin page
        List<OrderHeader> GetSubmittedUnconfirmedOrders();

        Guid GetUserIdForControlNumber(int controlNumber);

		PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, Core.Models.Paging.PagingModel paging);
	}
}
