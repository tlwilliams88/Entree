using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
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
        List<Order> GetCustomerOrders(UserSelectedContext catalogInfo);
        Order GetOrder(string branchId, string invoiceNumber);
		UserActiveCartModel GetUserActiveCart(Guid userId);
		void SaveUserActiveCart(Guid userId, Guid cartId);
	}
}
