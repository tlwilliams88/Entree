using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyOrderedListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        long Save(UserProfile user,
                  UserSelectedContext catalogInfo,
                  string itemNumber,
                  bool each,
                  string catalogId);

        void PostRecentOrder(UserProfile user, 
                             UserSelectedContext catalogInfo,
                             RecentNonBEKList list);

        void DeleteRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo, RecentlyOrderedListDetail details);
        void DeleteOldRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo, long headerId);

        void DeleteAll(UserProfile user,
                       UserSelectedContext catalogInfo);
    }
}