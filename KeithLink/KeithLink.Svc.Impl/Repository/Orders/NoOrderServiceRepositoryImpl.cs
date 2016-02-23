﻿using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders
{
	public class NoOrderServiceRepositoryImpl: IOrderServiceRepository
	{
		public NoOrderServiceRepositoryImpl()
        {
		}

		public string ReadLatestUpdatedDate(UserSelectedContext catalogInfo)
        {
            throw new NotImplementedException();
		}

        public List<OrderHistoryFile> GetLastFiveOrderHistory(UserSelectedContext catalogInfo, string itemNumber)
        {
            throw new NotImplementedException();
        }


        public List<Order> GetCustomerOrders(Guid userId, UserSelectedContext catalogInfo)
        {
            throw new NotImplementedException();
        }

        public Order GetOrder(string branchId, string invoiceNumber) {
            throw new NotImplementedException();
        }

        public List<Order> GetOrderHeaderInDateRange(UserSelectedContext customerInfo, DateTime startDate, DateTime endDate) {
            throw new NotImplementedException();
        }

        public OrderTotalByMonth GetOrderTotalByMonth( UserSelectedContext customerInfo, int numberOfMonths ) {
            throw new NotImplementedException();
        }

        public UserActiveCartModel GetUserActiveCart(UserSelectedContext catalogInfo, Guid userId) {
            throw new NotImplementedException();
        }

        public void SaveOrderHistory(OrderHistoryFile historyFile, bool isSpecialOrder)
        {
            throw new NotImplementedException();
        }

		public void SaveUserActiveCart(UserSelectedContext catalogInfo, Guid userId, Guid cartId)
		{
			throw new NotImplementedException();
		}


        public List<OrderHeader> GetSubmittedUnconfirmedOrders()
        {
            throw new NotImplementedException();
        }


        public Guid GetUserIdForControlNumber(int controlNumber)
        {
            throw new NotImplementedException();
        }


		public Core.Models.Paging.PagedResults<Order> GetPagedOrders(Guid userId, UserSelectedContext customerInfo, Core.Models.Paging.PagingModel paging)
		{
			throw new NotImplementedException();
		}

        #region IOrderServiceRepository Members


        public void UpdateRelatedOrderNumber(string childOrderNumber, string parentOrderNumber) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
