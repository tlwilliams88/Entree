﻿using KeithLink.Svc.Core.Models.Orders;
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

        List<Order> GetCustomerOrders(Guid userId, UserSelectedContext catalogInfo);

        Order GetOrder(string branchId, string invoiceNumber);

        List<Order> GetOrderHeaderInDateRange(Guid userId, UserSelectedContext customerInfo, DateTime startDate, DateTime endDate);
        
        UserActiveCartModel GetUserActiveCart(Guid userId);

		void SaveUserActiveCart(Guid userId, Guid cartId);

        void SaveOrderHistory(OrderHistoryFile historyFile);
	}
}
