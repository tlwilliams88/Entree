using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class InventoryValuationDetailExtensions {
        public static ListItemModel ToWebModel(this InventoryValuationListDetail value) {
            return new ListItemModel() {
                ListItemId = value.Id,
                Type = ListType.InventoryValuation,
                ItemNumber = value.ItemNumber,
                CustomInventoryItemId = value.CustomInventoryItemId ?? 0,
                Quantity = value.Quantity,
                Position = value.LineNumber,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId,
                Active = true
            };
        }

        public static InventoryValuationListDetail ToInventoryValuationListDetail(this ListItemModel value, long headerId = 0) {
            return new InventoryValuationListDetail() {
                Active = true,
                CatalogId = value.CatalogId,
                CustomInventoryItemId = value.CustomInventoryItemId,
                Each = value.Each,
                Id = value.ListItemId,
                ItemNumber = value.ItemNumber,
                LineNumber = value.Position,
                HeaderId = headerId,
                Quantity = value.Quantity
            };
        }
    }
}
