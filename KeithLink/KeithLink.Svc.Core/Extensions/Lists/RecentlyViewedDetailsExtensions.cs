using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class RecentlyViewedDetailsExtensions {
        public static ListItemModel ToWebModel(this RecentlyViewedListDetail value) {
            return new ListItemModel {
                Active = true,
                ListItemId = value.Id,
                Type = ListType.RecentlyViewed,
                ItemNumber = value.ItemNumber,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId,
                Position = value.LineNumber
            };
        }

        public static RecentlyViewedListDetail ToRecentlyViewedDetailList(this ListItemModel model, long headerId = 0) {
            return new RecentlyViewedListDetail {
                CatalogId = model.CatalogId,
                Each = model.Each ?? false,
                Id = model.ListItemId,
                ItemNumber = model.ItemNumber,
                HeaderId = headerId,
                LineNumber = model.Position
            };
        }
    }
}