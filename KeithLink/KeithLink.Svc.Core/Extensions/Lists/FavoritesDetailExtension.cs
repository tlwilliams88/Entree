﻿using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Favorites;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class FavoritesDetailExtension {
        public static ListItemModel ToWebModel(this FavoritesListDetail value) {
            return new ListItemModel() {
                Active = value.Active,
                ListItemId = value.Id,
                Type = ListType.Favorite,
                ItemNumber = value.ItemNumber,
                Position = value.LineNumber,
                Label = value.Label,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId
            };
        }

        public static FavoritesListDetail ToFavoritesListDetail(this ListItemModel model, long headerId = 0) {
            FavoritesListDetail item = new FavoritesListDetail() {
                Active = model.Active,
                CatalogId = model.CatalogId,
                Each = model.Each ?? false,
                Id = model.ListItemId,
                ItemNumber = model.ItemNumber,
                LineNumber = model.Position,
                Label = model.Label,
                HeaderId = headerId
            };

            return item;
        }
    }
}
