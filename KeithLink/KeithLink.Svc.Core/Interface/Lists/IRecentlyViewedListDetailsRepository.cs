using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyViewedListDetailsRepository {
        List<RecentlyViewedListDetail> GetRecentlyViewedDetails(long parentHeaderId);

        long Save(RecentlyViewedListDetail details);

        void DeleteRecentlyViewed(long detailId);
        void DeleteOldRecentlyViewed(long headerId, int numberToKeep = 7);
    }
}