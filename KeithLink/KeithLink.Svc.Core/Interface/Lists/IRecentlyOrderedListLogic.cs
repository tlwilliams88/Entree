using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyOrderedListLogic : IBaseListLogic
    {
        List<string> GetRecentlyOrderedItemNumbers(UserProfile user, UserSelectedContext catalogInfo);

        void AddOrUpdateRecentlyOrdered(UserProfile user,
            UserSelectedContext catalogInfo,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);

        void DeleteRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo);

        void AddRecentlyOrderedItems(UserProfile user, UserSelectedContext catalogInfo, RecentNonBEKList newlist);
    }
}
