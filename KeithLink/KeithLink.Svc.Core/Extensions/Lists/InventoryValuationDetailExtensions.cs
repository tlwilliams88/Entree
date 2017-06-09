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
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId
            };
        }
    }
}
