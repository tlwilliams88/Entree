using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecommendedItemsListLogic : IBaseListLogic
    {
        List<string> GetRecommendedItemNumbers(UserProfile user, UserSelectedContext catalogInfo);

        void AddOrUpdateRecommendedItem(UserSelectedContext catalogInfo,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);
    }
}
