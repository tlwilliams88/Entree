using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class MandatoryItemDetailExtensions {
        public static ListItemModel ToWebModel(this MandatoryItemsListDetail value) {
            return new ListItemModel() {
                ListItemId = value.Id,
                Type = ListType.Mandatory,
                ItemNumber = value.ItemNumber,
                Each = value.Each ?? false,
                Quantity = value.Quantity,
                CatalogId = value.CatalogId,
                Position = value.LineNumber,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc
            };
        }

        public static MandatoryItemsListDetail ToMandatoryItemsListDetail(this ListItemModel model, long headerId = 0) {
            MandatoryItemsListDetail item = new MandatoryItemsListDetail() {
                Active = true,
                CatalogId = model.CatalogId,
                Each = model.Each ?? false,
                Id = model.ListItemId,
                Quantity = model.Quantity,
                ItemNumber = model.ItemNumber,
                HeaderId = headerId,
                LineNumber = model.Position
            };

            return item;
        }
    }
}
