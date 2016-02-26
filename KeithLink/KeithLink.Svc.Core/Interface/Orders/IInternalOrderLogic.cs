﻿using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders
{
	public interface IInternalOrderLogic
	{
		string ReadLatestUpdatedDate(UserSelectedContext catalogInfo);
        List<Core.Models.Orders.History.OrderHistoryFile> GetLastFiveOrderHistory( UserSelectedContext catalogInfo, string itemNumber );
        List<Core.Models.Orders.History.OrderHistoryHeader> GetCustomerOrderHistories(UserSelectedContext catalogInfo);
		UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId);
		void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId);
        List<OrderHeader> GetSubmittedUnconfirmedOrders();
        Guid GetUserIdForControlNumber(int controlNumber);
	}
}
