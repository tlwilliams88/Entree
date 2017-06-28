using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;

namespace KeithLink.Svc.Core.Extensions.Lists
{
    public static class RecommendedHeaderExtensions
    {
        #region attributes
        private const string LISTNAME_RECOMMENDED = "Recommended";
        #endregion

        #region methods
        public static ListModel ToListModel(this RecommendedItemsListHeader header)
        {
            return new ListModel
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.RecommendedItems,
                ListId = header.Id,
                Name = LISTNAME_RECOMMENDED,
                ReadOnly = true,
                Items = null
            };
        }

        public static ListModel ToListModel(this RecommendedItemsListHeader header, List<RecommendedItemsListDetail> items)
        {
            ListModel retVal = header.ToListModel();

            if (items != null) {
                retVal.Items = items.Select(i => i.ToWebModel())
                                    .OrderBy(i => i.Position)
                                    .ToList();
            }

            return retVal;
        }

        public static ListModel ToListModel(this RecommendedItemsListHeader header, List<ListItemModel> items)
        {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.OrderBy(i => i.Position)
                                .ToList();
            return retVal;
        }
        #endregion
    }
}
