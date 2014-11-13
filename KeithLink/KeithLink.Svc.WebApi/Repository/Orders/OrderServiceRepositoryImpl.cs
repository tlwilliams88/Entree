using KeithLink.Svc.Core.Interface.Orders;
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

        public List<Core.Models.Orders.History.OrderHistoryFile> GetLastFiveOrderHistory( UserSelectedContext catalogInfo, string itemNumber ) {
            return serviceClient.GetLastFiveOrderHistory( catalogInfo, itemNumber ).ToList();
        }
	}
}
