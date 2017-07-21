using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyViewedListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        void PostRecentView(UserProfile user, 
                            UserSelectedContext catalogInfo,
                            string itemNumber);

        void DeleteAll(UserProfile user,
                       UserSelectedContext catalogInfo);
    }
}