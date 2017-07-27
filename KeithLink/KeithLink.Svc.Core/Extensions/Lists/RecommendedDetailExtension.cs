using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;

namespace KeithLink.Svc.Core.Extensions.Lists
{
    public static class RecommendedDetailExtension
    {
        public static ListItemModel ToWebModel(this RecommendedItemsListDetail value)
        {
            return new ListItemModel()
            {
                Active = true,
                ListItemId = value.Id,
                Type = ListType.RecommendedItems,
                ItemNumber = value.ItemNumber,
                Each = value.Each ?? false,
                CatalogId = value.CatalogId,
                Position = value.LineNumber,
                ModifiedUtc = value.ModifiedUtc,
                CreatedUtc = value.CreatedUtc
            };
        }

        public static RecommendedItemsListDetail ToRecommendedItemsListDetail(this ListItemModel model, long headerId = 0)
        {
            RecommendedItemsListDetail item = new RecommendedItemsListDetail()
            {
                CatalogId = model.CatalogId,
                Each = model.Each ?? false,
                Id = model.ListItemId,
                ItemNumber = model.ItemNumber,
                HeaderId = headerId,
                LineNumber = model.Position
            };

            return item;
        }
    }
}
