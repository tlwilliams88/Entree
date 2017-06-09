using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IFavoriteListDetailRepository
    {
        List<FavoritesListDetail> GetFavoritesListDetails(long headerId);

        void SaveFavoriteListDetail(FavoritesListDetail model);
    }
}
