using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class RecentlyOrderedDetailsExtensions {
        public static ListItemModel ToWebModel(this RecentlyOrderedListDetail value) {
            return new ListItemModel {
                                         ListItemId = value.Id,
                                         Type = ListType.RecentlyOrdered,
                                         ItemNumber = value.ItemNumber,
                                         ModifiedUtc = value.ModifiedUtc,
                                         CreatedUtc = value.CreatedUtc,
                                         Each = value.Each ?? false,
                                         CatalogId = value.CatalogId,
                                         Position = value.LineNumber
                                     };
        }

        public static RecentlyOrderedListDetail ToRecentlyOrderedDetailList(this ListItemModel model, long headerId = 0) {
            return new RecentlyOrderedListDetail {
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