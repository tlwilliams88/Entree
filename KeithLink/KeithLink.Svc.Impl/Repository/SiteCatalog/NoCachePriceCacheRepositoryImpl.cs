using KeithLink.Svc.Core.Interface.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
	public class NoCachePriceCacheRepositoryImpl: IPriceCacheRepository
	{
		public void AddItem(string branchId, string customerNumber, string itemNumber, double casePrice, double packagePrice)
		{
			//Do Nothing
		}

		public void ResetAllItems()
		{
			//Do Nothing
		}

		public void ResetItemsByCustomer(string branchId, string customerNumber)
		{
			//Do Nothing
		}

		public void RemoveItem(string branchId, string customerNumber, string itemNumber)
		{
			//Do Nothing
		}

		public Core.Models.SiteCatalog.Price GetPrice(string branchId, string customerNumber, string itemNumber)
		{
			return null;
		}

		public void AddItem(Core.Models.SiteCatalog.Price price)
		{
			//Do Nothing
		}
	}
}
