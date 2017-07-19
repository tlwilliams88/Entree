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
            ListModel retVal = new ListModel();

            retVal.BranchId = header.BranchId;
            retVal.CustomerNumber = header.CustomerNumber;
            retVal.IsContractList = false;
            retVal.IsFavorite = false;
            retVal.IsWorksheet = false;
            retVal.IsReminder = false;
            retVal.IsMandatory = false;
            retVal.IsRecommended = false;
            retVal.IsCustomInventory = false;
            retVal.Type = ListType.Custom;
            
            if(shares != null) {
                retVal.SharedWith = shares.Select(s => s.CustomerNumber)
                                          .ToList();
            }
            
            retVal.ListId = header.Id;
            retVal.Name = (header != null && header.Name != null) ? header.Name : "(No set name)";
            retVal.ReadOnly = !header.CustomerNumber.Equals(catalogInfo.CustomerId);
            retVal.IsSharing = shares != null &&
                               (shares.Any() &&
                                header.CustomerNumber.Equals(catalogInfo.CustomerId) &&
                                header.BranchId.Equals(catalogInfo.BranchId, StringComparison.CurrentCultureIgnoreCase));
            retVal.IsShared = !header.CustomerNumber.Equals(catalogInfo.CustomerId);

            return retVal;
        }

        public static ListModel ToListModel(this CustomListHeader header, UserSelectedContext catalogInfo, List<CustomListShare> shares, 
                                            List<CustomListDetail> items) {
            ListModel retVal = header.ToListModel(catalogInfo, shares);

            if (items != null) {
                retVal.Items = items.Select(i => i.ToWebModel())
                                    .OrderBy(l => l.Position)
                                    .ToList();
            }

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
