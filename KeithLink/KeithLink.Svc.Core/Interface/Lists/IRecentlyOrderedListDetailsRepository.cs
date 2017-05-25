using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRecentlyOrderedListDetailsRepository
    {
        List<RecentlyOrderedListDetail> GetRecentlyOrderedDetails(long parentHeaderId);

        void AddOrUpdateRecentlyOrdered(string userId,
            string customerNumber,
            string branchId,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);

        void DeleteRecentlyOrdered(string userId,
            string customerNumber,
            string branchId);
    }
}
