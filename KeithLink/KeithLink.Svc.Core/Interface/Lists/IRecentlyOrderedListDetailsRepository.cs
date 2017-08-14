using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyOrderedListDetailsRepository {
        List<RecentlyOrderedListDetail> GetRecentlyOrderedDetails(long parentHeaderId);

        long Save(RecentlyOrderedListDetail details);

        void DeleteRecentlyOrdered(long detailId);
        void DeleteOldRecentlyOrdered(long headerId, int numberToKeep = 7);
    }
}