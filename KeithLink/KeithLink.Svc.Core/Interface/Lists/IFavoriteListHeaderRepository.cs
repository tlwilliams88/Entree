using System;

using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IFavoriteListHeaderRepository
    {
        FavoritesListHeader GetFavoritesList(Guid userId, UserSelectedContext catalogInfo);

        void SaveFavoriteListHeader(FavoritesListHeader model);
    }
}
