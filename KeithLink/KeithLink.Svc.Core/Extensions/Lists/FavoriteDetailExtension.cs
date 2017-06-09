using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class FavoriteDetailExtension {
        public static ListItemModel ToWebModel(this FavoritesListDetail value) {
            return new ListItemModel() {
                ListItemId = value.Id,
                Type = ListType.Favorite,
                ItemNumber = value.ItemNumber,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId
            };
        }
    }
}
