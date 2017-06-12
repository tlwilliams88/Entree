using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class CustomDetailExtensions {
        public static ListItemModel ToWebModel(this CustomListDetail value) {
            return new ListItemModel() {
                ListItemId = value.Id,
                Type = ListType.Custom,
                ItemNumber = value.ItemNumber,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId
            };
        }
    }
}
