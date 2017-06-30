using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class ReminderDetailExtensions {
        public static ListItemModel ToWebModel(this ReminderItemsListDetail value) {
            return new ListItemModel() {
                ListItemId = value.Id,
                Type = ListType.Reminder,
                ItemNumber = value.ItemNumber,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId,
                Position = value.LineNumber
            };
        }

        public static ReminderItemsListDetail ToReminderItemsListDetail(this ListItemModel model, long headerId = 0) {
            return new ReminderItemsListDetail() {
                Active = true,
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
