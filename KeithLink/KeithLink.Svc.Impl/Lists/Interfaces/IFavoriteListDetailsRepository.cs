using System.Collections.Generic;

using Entree.Core.Models.Lists.Favorites;

namespace Entree.Core.Interface.Lists {
    public interface IFavoriteListDetailsRepository
    {
        void DeleteFavoriteListDetail(long id);

        FavoritesListDetail GetFavoriteDetail(long id);

        List<FavoritesListDetail> GetFavoritesListDetails(long headerId);

        long SaveFavoriteListDetail(FavoritesListDetail model);
    }
}
