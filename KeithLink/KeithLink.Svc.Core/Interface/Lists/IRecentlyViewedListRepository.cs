using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRecentlyViewedListRepository
    {
        List<ListModel> GetRecentlyViewedList(string userId, UserSelectedContext catalogInfo, bool headerOnly);

        void AddOrUpdateRecentlyViewed(string userId,
            string customerNumber,
            string branchId,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);
    }
}
