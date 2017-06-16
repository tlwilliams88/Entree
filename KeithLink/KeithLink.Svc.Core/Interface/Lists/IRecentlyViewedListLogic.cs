using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyViewedListLogic : IBaseListLogic
    {
        List<string> GetRecentlyViewedItemNumbers(UserProfile user, UserSelectedContext catalogInfo);

        void AddOrUpdateRecentlyViewed(UserProfile user,
            UserSelectedContext catalogInfo,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);

        void DeleteRecentlyViewed(UserProfile user, UserSelectedContext catalogInfo);

        List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);
    }
}
