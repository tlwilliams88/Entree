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
	public class OrderServiceRepositoryImpl: IOrderServiceRepository {
        #region attributes
        private com.benekeith.OrderService.IOrderService serviceClient;
        #endregion

        #region ctor
        public OrderServiceRepositoryImpl(com.benekeith.OrderService.IOrderService serviceClient) {
			this.serviceClient = serviceClient;
		}
        #endregion

        #region methods
        public List<Order> GetCustomerOrders(Guid userId, UserSelectedContext catalogInfo) {
            return serviceClient.GetCustomerOrders(userId, catalogInfo).ToList();
        }

        public List<OrderHistoryFile> GetLastFiveOrderHistory(UserSelectedContext catalogInfo, string itemNumber) {
            return serviceClient.GetLastFiveOrderHistory(catalogInfo, itemNumber).ToList();
        }

        public Order GetOrder(string branchId, string invoiceNumber) {
            return serviceClient.GetOrder(branchId, invoiceNumber);
        }

        public UserActiveCartModel GetUserActiveCart(Guid userId) {
            return serviceClient.GetUserActiveCart(userId);
        }

        public DateTime? ReadLatestUpdatedDate(UserSelectedContext catalogInfo) {
            return serviceClient.ReadLatestOrderModifiedDateForCustomer(catalogInfo);
        }

        public void SaveUserActiveCart(Guid userId, Guid cartId) {
            serviceClient.SaveUserActiveCart(userId, cartId);
        }
        #endregion


		public List<Order> GetOrderHeaderInDateRange(Guid userId, UserSelectedContext customerInfo, DateTime startDate, DateTime endDate)
		{
			return serviceClient.GetOrderHeaderInDateRange(userId, customerInfo, startDate, endDate).ToList();
		}
	}
}
