using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IFavoriteListDetailRepository
    {
        void AddOrUpdateFavorite(string userId, string customerNumber, string branchId,
                                 string itemNumber, bool each, string catalogId,
                                 bool active);

        List<FavoritesListDetail> GetFavoritesListDetails(long headerId);
    }
}
