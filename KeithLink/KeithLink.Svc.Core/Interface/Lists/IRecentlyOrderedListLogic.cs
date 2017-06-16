using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyOrderedListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        void Save(UserProfile user,
                  UserSelectedContext catalogInfo,
                  string itemNumber,
                  bool each,
                  string catalogId);

        void DeleteRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo, RecentlyOrderedListDetail details);
        void DeleteOldRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo, long headerId);
    }
}