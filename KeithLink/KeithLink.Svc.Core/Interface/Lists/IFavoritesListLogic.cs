using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IFavoritesListLogic : IBaseListLogic
    {
        List<string> GetFavoritedItemNumbers(UserProfile user, UserSelectedContext catalogInfo);

        ListModel GetFavoritesList(string userId, UserSelectedContext catalogInfo, bool headerOnly);

        void AddOrUpdateFavorite(UserProfile user,
            UserSelectedContext catalogInfo,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);

    }
}
