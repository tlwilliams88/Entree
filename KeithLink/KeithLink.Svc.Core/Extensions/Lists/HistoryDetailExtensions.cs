using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.History;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class HistoryDetailExtensions {
        public static ListItemModel ToWebModel(this HistoryListDetail value) {
            return new ListItemModel()
            {
                ListItemId = value.Id,
                Type = ListType.Worksheet,
                ItemNumber = value.ItemNumber,
                Position = value.LineNumber,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId
            };
        }
    }
}
