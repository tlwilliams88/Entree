using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.WebApi.Repository.Orders
{
	public class OrderServiceRepositoryImpl: IOrderServiceRepository
	{
		private com.benekeith.OrderService.IOrderService serviceClient;

		public OrderServiceRepositoryImpl(com.benekeith.OrderService.IOrderService serviceClient)
		{
			this.serviceClient = serviceClient;
		}

		public DateTime? ReadLatestUpdatedDate(UserSelectedContext catalogInfo)
		{
			return serviceClient.ReadLatestOrderModifiedDateForCustomer(catalogInfo);
		}

        public List<OrderHistoryFile> GetLastFiveOrderHistory( UserSelectedContext catalogInfo, string itemNumber ) {
            return serviceClient.GetLastFiveOrderHistory( catalogInfo, itemNumber ).ToList();
        }

        public List<Order> GetCustomerOrders(UserSelectedContext catalogInfo)
        {
            return serviceClient.GetCustomerOrders(catalogInfo).ToList();
        }

        public Order GetOrder(string branchId, string invoiceNumber) {
            return serviceClient.GetOrder(branchId, invoiceNumber);
        }

        public UserActiveCartModel GetUserActiveCart(Guid userId)
		{
			return serviceClient.GetUserActiveCart(userId);
		}

		public void SaveUserActiveCart(Guid userId, Guid cartId)
		{
			serviceClient.SaveUserActiveCart(userId, cartId);
		}
	}
}
