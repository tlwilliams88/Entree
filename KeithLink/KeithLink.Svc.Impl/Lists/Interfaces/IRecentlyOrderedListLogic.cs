using System.Collections.Generic;

using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.RecentlyOrdered;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface IRecentlyOrderedListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        void PostRecentOrder(UserProfile user, 
                             UserSelectedContext catalogInfo,
                             RecentNonBEKList list);

        void DeleteAll(UserProfile user,
                       UserSelectedContext catalogInfo);
    }
}