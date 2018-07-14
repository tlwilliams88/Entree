using System.Collections.Generic;

using Entree.Core.Models.Lists.RecentlyOrdered;

namespace Entree.Core.Interface.Lists {
    public interface IRecentlyOrderedListDetailsRepository {
        List<RecentlyOrderedListDetail> GetRecentlyOrderedDetails(long parentHeaderId);

        long Save(RecentlyOrderedListDetail details);

        void DeleteRecentlyOrdered(long detailId);
        void DeleteOldRecentlyOrdered(long headerId, int numberToKeep = 7);
    }
}