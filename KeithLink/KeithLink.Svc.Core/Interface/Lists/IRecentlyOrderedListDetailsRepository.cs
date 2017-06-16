using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyOrderedListDetailsRepository {
        List<RecentlyOrderedListDetail> GetRecentlyOrderedjetails(long parentHeaderId);

        long Save(RecentlyOrderedListDetail details);

        void DeleteRecentlyOrdered(RecentlyOrderedListDetail details);
        void DeleteOldRecentlyOrdered(long headerId, int numberToKeep = 7);
    }
}