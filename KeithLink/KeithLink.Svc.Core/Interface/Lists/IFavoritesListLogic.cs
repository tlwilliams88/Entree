using System;
using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IFavoritesListLogic : IBaseListLogic {
        List<string> GetFavoritedItemNumbers(UserProfile user, UserSelectedContext catalogInfo);

        ListModel GetFavoritesList(Guid userId, UserSelectedContext catalogInfo, bool headerOnly);

        void Save(UserProfile user, UserSelectedContext catalogInfo, FavoritesListDetail model);

        ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);
    }
}
