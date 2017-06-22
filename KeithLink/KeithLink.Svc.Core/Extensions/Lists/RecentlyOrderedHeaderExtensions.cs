using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class RecentlyOrderedHeaderExtensions {
        #region attributes
        private const string LISTNAME_RECENTLY_ORDERED = "Recently Ordered";
        #endregion

        #region methods
        public static ListModel ToListModel(this RecentlyOrderedListHeader header) {
            return new ListModel {
                                     BranchId = header.BranchId,
                                     IsContractList = false,
                                     IsFavorite = false,
                                     IsWorksheet = false,
                                     IsReminder = false,
                                     IsMandatory = false,
                                     IsRecommended = false,
                                     IsCustomInventory = false,
                                     Type = ListType.RecentlyOrdered,
                                     ListId = header.Id,
                                     Name = LISTNAME_RECENTLY_ORDERED,
                                     ReadOnly = true,
                                     Items = null
                                 };
        }

        public static ListModel ToListModel(this RecentlyOrderedListHeader header, List<RecentlyOrderedListDetail> items) {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.Select(i => i.ToWebModel())
                                .OrderBy(i => i.Position)
                                .ToList();
            return retVal;
        }

        public static ListModel ToListModel(this RecentlyOrderedListHeader header, List<ListItemModel> items) {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.OrderBy(i => i.Position)
                                .ToList();
            return retVal;
        }
        #endregion
    }
}