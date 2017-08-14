using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class InventoryValuationHeaderExtensions {
        public static ListModel ToListModel(this InventoryValuationListHeader header) {
            return new ListModel() {
                BranchId = header.BranchId,
                CustomerNumber = header.CustomerNumber,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.InventoryValuation,
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = false
            };
        }

        public static ListModel ToListModel(this InventoryValuationListHeader header, List<InventoryValuationListDetail> items) {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.Select(i => i.ToWebModel())
                                .OrderBy(i => i.Position)
                                .ToList();
            return retVal;
        }

        public static ListModel ToListModel(this InventoryValuationListHeader header, List<ListItemModel> items) {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.OrderBy(i => i.Position)
                                .ToList();
            return retVal;
        }
    }
}
