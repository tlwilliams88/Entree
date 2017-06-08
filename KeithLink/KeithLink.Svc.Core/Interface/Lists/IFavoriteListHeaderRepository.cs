using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IFavoriteListHeaderRepository
    {
        FavoritesListHeader GetFavoritesList(string userId, UserSelectedContext catalogInfo);
    }
}
