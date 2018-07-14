using System.Collections.Generic;

using Entree.Core.Models.Lists.RecentlyViewed;

namespace Entree.Core.Interface.Lists {
    public interface IRecentlyViewedListDetailsRepository {
        List<RecentlyViewedListDetail> GetRecentlyViewedDetails(long parentHeaderId);

        long Save(RecentlyViewedListDetail details);

        void DeleteRecentlyViewed(long detailId);
        void DeleteOldRecentlyViewed(long headerId, int numberToKeep = 7);
    }
}