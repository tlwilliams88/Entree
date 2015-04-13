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

		public UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId)
		{
            return serviceClient.GetUserActiveCart(catalogInfo, userId);
        }

        public List<Order> GetOrderHeaderInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            return serviceClient.GetOrderHeaderInDateRange(customerInfo, startDate, endDate).ToList();
        }

        public DateTime? ReadLatestUpdatedDate(UserSelectedContext catalogInfo) {
            return serviceClient.ReadLatestOrderModifiedDateForCustomer(catalogInfo);
        }

		public void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId)
		{
            serviceClient.SaveUserActiveCart(catalogInfo, userId, cartId);
        }

        public void SaveOrderHistory(OrderHistoryFile historyFile) {
            serviceClient.SaveOrderHistory(historyFile);
        }

        public List<OrderHeader> GetSubmittedUnconfirmedOrders()
        {
            return serviceClient.GetSubmittedUnconfirmedOrders().ToList();
        }

        public Guid GetUserIdForControlNumber(int controlNumber)
        {
            return serviceClient.GetUserIdForControlNumber(controlNumber);
        }
        #endregion


		public Core.Models.Paging.PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, Core.Models.Paging.PagingModel paging)
		{
			return serviceClient.GetPagedOrders(userId, customerInfo, paging);
		}
	}
}
