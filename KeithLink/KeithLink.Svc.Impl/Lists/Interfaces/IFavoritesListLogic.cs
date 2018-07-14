using System;
using System.Collections.Generic;

using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.Favorites;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface IFavoritesListLogic : IBaseListLogic {
        List<string> GetFavoritedItemNumbers(UserProfile user, UserSelectedContext catalogInfo);

        ListModel GetFavoritesList(Guid userId, UserSelectedContext catalogInfo, bool headerOnly);

        void Save(UserProfile user, UserSelectedContext catalogInfo, FavoritesListDetail model);

        ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);

        long CreateList(UserProfile user, UserSelectedContext catalogInfo);
    }
}
