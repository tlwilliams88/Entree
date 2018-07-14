using System.Collections.Generic;

using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.RecentlyViewed;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface IRecentlyViewedListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        void PostRecentView(UserProfile user, 
                            UserSelectedContext catalogInfo,
                            string itemNumber);

        void DeleteAll(UserProfile user,
                       UserSelectedContext catalogInfo);
    }
}