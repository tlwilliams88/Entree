using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.Lists.CustomListShares;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class CustomHeaderExtensions {
        public static ListModel ToListModel(this CustomListHeader header, UserSelectedContext catalogInfo, List<CustomListShare> shares) {
            return new ListModel() {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Custom,
                SharedWith = shares?.Select(s => s.CustomerNumber).ToList(),
                ListId = header.Id,
                Name = header.Name,
                ReadOnly = !header.CustomerNumber.Equals(catalogInfo.CustomerId),
                IsSharing = shares != null && 
                                     (shares.Any() && 
                                      header.CustomerNumber.Equals(catalogInfo.CustomerId) &&
                                      header.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase)),
                IsShared = !header.CustomerNumber.Equals(catalogInfo.CustomerId),
                Items = null
            };
        }

        public static ListModel ToListModel(this CustomListHeader header, UserSelectedContext catalogInfo, List<CustomListShare> shares, 
                                            List<CustomListDetail> items) {
            ListModel retVal = header.ToListModel(catalogInfo, shares);
            retVal.Items = items.Select(i => i.ToWebModel())
                                .OrderBy(l => l.Position)
                                .ToList();
            return retVal;
        }

        public static ListModel ToListModel(this CustomListHeader header, UserSelectedContext catalogInfo, List<CustomListShare> shares,
                                            List<ListItemModel> items) {
            ListModel retVal = header.ToListModel(catalogInfo, shares);
            retVal.Items = items.OrderBy(l => l.Position)
                                .ToList();
            return retVal;
        }
    }
}
