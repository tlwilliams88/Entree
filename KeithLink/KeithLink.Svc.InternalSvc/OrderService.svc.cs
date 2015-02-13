using CommerceServer.Core;
using CommerceServer.Core.Orders;
using CommerceServer.Core.Runtime.Orders;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.InternalSvc.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc {
	[GlobalErrorBehaviorAttribute(typeof(ErrorHandler))]
	public class OrderService : IOrderService {
        #region attributes
        private readonly IEventLogRepository _eventLog;
		private readonly IInternalOrderLogic _orderLogic;
        private readonly IInternalOrderHistoryLogic _historyLogic;
        #endregion

        #region ctor
        public OrderService(IEventLogRepository eventLog, IInternalOrderLogic orderLogic, IInternalOrderHistoryLogic orderHistoryLogic) {
            _eventLog = eventLog;
			_orderLogic = orderLogic;
            _historyLogic = orderHistoryLogic;
		}
        #endregion

        #region methods
        public DateTime? ReadLatestOrderModifiedDateForCustomer(UserSelectedContext catalogInfo) {
			return _orderLogic.ReadLatestUpdatedDate(catalogInfo);
		}

        public List<OrderHistoryFile> GetLastFiveOrderHistory(UserSelectedContext catalogInfo, string itemNumber ) {
            return _orderLogic.GetLastFiveOrderHistory( catalogInfo, itemNumber );
        }

        public List<Order> GetCustomerOrders(Guid userId, UserSelectedContext catalogInfo) {
            return _historyLogic.GetOrders(userId, catalogInfo);
        }

        public Order GetOrder(string branchId, string invoiceNumber) {
            return _historyLogic.GetOrder(branchId, invoiceNumber);
        }

        public List<Order> GetOrderHeaderInDateRange(Guid userId, UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            return _historyLogic.GetOrderHeaderInDateRange(userId, customerInfo, startDate, endDate);
        }

		public UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId)
		{
			return _orderLogic.GetUserActiveCart(catalogInfo, userId);
        }

        public void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId) {
            _orderLogic.SaveUserActiveCart(catalogInfo, userId, cartId);
        }

        public void SaveOrderHistory(OrderHistoryFile historyFile) {
            _historyLogic.SaveOrder(historyFile);
        }

        public List<OrderHeader> GetSubmittedUnconfirmedOrders()
        {
            return _orderLogic.GetSubmittedUnconfirmedOrders();
        }

        public Guid GetUserIdForControlNumber(int controlNumber)
        {
            return _orderLogic.GetUserIdForControlNumber(controlNumber);
        }
		


		public Core.Models.Paging.PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, Core.Models.Paging.PagingModel paging)
		{
			return _historyLogic.GetPagedOrders(userId, customerInfo, paging);
		}
		#endregion
	}		
}
