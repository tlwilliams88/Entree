using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyOrderedListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        void PostRecentOrder(UserProfile user, 
                             UserSelectedContext catalogInfo,
                             RecentNonBEKList list);

        void DeleteAll(UserProfile user,
                       UserSelectedContext catalogInfo);
    }
}