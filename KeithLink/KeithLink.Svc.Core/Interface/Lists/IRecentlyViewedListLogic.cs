using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyViewedListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        long Save(UserProfile user,
                  UserSelectedContext catalogInfo,
                  string itemNumber,
                  bool each,
                  string catalogId);

        void PostRecentOrder(UserProfile user, 
                             UserSelectedContext catalogInfo,
                             RecentNonBEKList list);

        void DeleteRecentlyViewed(UserProfile user, UserSelectedContext catalogInfo, RecentlyViewedListDetail details);
        void DeleteOldRecentlyViewed(UserProfile user, UserSelectedContext catalogInfo, long headerId);

        void DeleteAll(UserProfile user,
                       UserSelectedContext catalogInfo);
    }
}