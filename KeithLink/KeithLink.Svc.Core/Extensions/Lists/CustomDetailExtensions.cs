﻿using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class CustomDetailExtensions {
        public static ListItemModel ToWebModel(this CustomListDetail value) {
            return new ListItemModel() {
                ListItemId = value.Id,
                Type = ListType.Custom,
                ItemNumber = value.ItemNumber,
                Position = value.LineNumber,
                Label = value.Label,
                ParLevel = value.Par,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId,
                Active = value.Active,
                CustomInventoryItemId = value.CustomInventoryItemId ?? 0
            };
        }

        public static CustomListDetail ToCustomListDetail(this ListItemModel value, long headerId = 0) {
            return new CustomListDetail() {
                Active = true,
                CatalogId = value.CatalogId,
                CustomInventoryItemId = value.CustomInventoryItemId,
                Each = value.Each,
                Id = value.ListItemId,
                ItemNumber = value.ItemNumber,
                LineNumber = value.Position,
                Label = value.Label,
                Par = value.ParLevel,
                HeaderId = headerId
            };
        }
    }
}
