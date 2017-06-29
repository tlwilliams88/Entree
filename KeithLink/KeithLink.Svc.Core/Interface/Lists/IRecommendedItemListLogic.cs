using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecommendedItemsListLogic : IBaseListLogic {
        long CreateList(UserSelectedContext catalogInfo);

        List<string> GetRecommendedItemNumbers(UserProfile user, UserSelectedContext catalogInfo);

        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        void SaveDetail(UserSelectedContext catalogInfo, RecommendedItemsListDetail detail);

        void DeleteRecommendedItems(UserProfile user, UserSelectedContext catalogInfo);
    }
}
