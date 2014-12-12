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

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PipelineService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select PipelineService.svc or PipelineService.svc.cs at the Solution Explorer and start debugging.
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

        public List<Order> GetCustomerOrders(UserSelectedContext catalogInfo) {
            return _historyLogic.GetOrders(catalogInfo);
        }

        public Order GetOrder(string branchId, string invoiceNumber) {
            return _historyLogic.GetOrder(branchId, invoiceNumber);
        }
        #endregion
    }
}
