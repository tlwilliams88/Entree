using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IFavoriteListDetailsRepository
    {
        void DeleteFavoriteListDetail(long id);

        List<FavoritesListDetail> GetFavoritesListDetails(long headerId);

        long SaveFavoriteListDetail(FavoritesListDetail model);
    }
}
