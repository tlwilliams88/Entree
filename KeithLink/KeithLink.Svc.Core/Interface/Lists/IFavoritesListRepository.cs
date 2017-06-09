using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IFavoritesListRepository
    {
        List<ListModel> GetFavoritesList(string userId, UserSelectedContext catalogInfo, bool headerOnly);

        void AddOrUpdateFavorite(string userId,
            string customerNumber,
            string branchId,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);
    }
}
