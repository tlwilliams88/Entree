using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyViewedListDetailsRepository {
        List<RecentlyViewedListDetail> GetRecentlyViewedDetails(long parentHeaderId);

        long Save(RecentlyViewedListDetail details);

        void DeleteRecentlyViewed(RecentlyViewedListDetail details);
        void DeleteOldRecentlyViewed(long headerId, int numberToKeep = 7);
    }
}