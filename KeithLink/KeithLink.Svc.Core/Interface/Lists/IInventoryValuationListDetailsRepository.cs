using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IInventoryValuationListDetailsRepository
    {
        List<InventoryValuationListDetail> GetInventoryValuationDetails(long parentHeaderId);

        void AddOrUpdateRecommendedItem(string customerNumber,
                                string branchId,
                                long listId,
                                string listName,
                                string itemNumber,
                                bool each,
                                decimal quantity,
                                string catalogId,
                                bool active);
    }
}
