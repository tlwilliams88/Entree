using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IFavoriteListDetailsRepository
    {
        void DeleteFavoriteListDetail(long id);

        FavoritesListDetail GetFavoriteDetail(long id);

        List<FavoritesListDetail> GetFavoritesListDetails(long headerId);

        long SaveFavoriteListDetail(FavoritesListDetail model);
    }
}
