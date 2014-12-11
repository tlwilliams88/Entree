using KeithLink.Svc.Core.Models.Orders;
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
        List<Core.Models.Orders.History.OrderHistoryFile> GetLastFiveOrderHistory( UserSelectedContext catalogInfo, string itemNumber );
        List<Core.Models.Orders.Order> GetCustomerOrders(UserSelectedContext catalogInfo);
		UserActiveCartModel GetUserActiveCart(Guid userId);
		void SaveUserActiveCart(Guid userId, Guid cartId);
	}
}
