using System;

using Entree.Core.Models.Lists.Favorites;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface IFavoriteListHeadersRepository
    {
        FavoritesListHeader GetFavoritesList(Guid userId, UserSelectedContext catalogInfo);

        long SaveFavoriteListHeader(FavoritesListHeader model);
    }
}
