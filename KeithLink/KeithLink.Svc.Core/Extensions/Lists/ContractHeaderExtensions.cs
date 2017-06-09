using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class ContractHeaderExtensions {
        #region attributes
        private const string LISTNAME_CONTRACT = "Contract - ";
        #endregion

        #region methods
        public static ListModel ToListModel(this ContractListHeader header, UserSelectedContext catalogInfo) {
            return new ListModel() {
                BranchId = header.BranchId,
                IsContractList = true,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Contract,
                ListId = header.Id,
                Name = $"{LISTNAME_CONTRACT}{header.ContractId}",
                ReadOnly = true,
                Items = null
            };
        }

        public static ListModel ToListModel(this ContractListHeader header, UserSelectedContext catalogInfo, List<ContractListDetail> items) {
            ListModel retVal = ToListModel(header, catalogInfo);
            retVal.Items = items.Select(i => i.ToWebModel())
                                .OrderBy(i => i.Position)
                                .ToList();

            return retVal;
        }

        public static ListModel ToListModel(this ContractListHeader header, UserSelectedContext catalogInfo, List<ListItemModel> items) {
            ListModel retVal = ToListModel(header, catalogInfo);
            retVal.Items = items.OrderBy(i => i.Position)
                                .ToList();

            return retVal;
        }
        #endregion
    }
}
