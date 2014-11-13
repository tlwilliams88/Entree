using KeithLink.Svc.Core.Interface.Orders;
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

		public DateTime? ReadLatestUpdatedDate(UserSelectedContext catalogInfo)
        {
            throw new NotImplementedException();
		}

        public List<Core.Models.Orders.History.OrderHistoryFile> GetLastFiveOrderHistory(UserSelectedContext catalogInfo, string itemNumber)
        {
            throw new NotImplementedException();
        }
	}
}
