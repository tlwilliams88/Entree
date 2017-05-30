using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface ICustomListDetailsRepository
    {
        List<InventoryValuationListDetail> GetCustomDetails(long parentHeaderId);

        void AddOrUpdateRecommendedItem(long listId,
                                string itemNumber,
                                bool each,
                                decimal par,
                                string catalogId,
                                bool active);
    }
}
