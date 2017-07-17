using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class ContractHeaderExtensions {
        #region attributes
        private const string LISTNAME_CONTRACT = "Contract - ";
        #endregion

        #region methods
        public static ListModel ToListModel(this ContractListHeader header) {
            return new ListModel() {
                BranchId = header.BranchId,
                CustomerNumber = header.CustomerNumber,
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
                HasContractItems = false,
                Items = new List<ListItemModel>()
            };
        }

        public static ListModel ToListModel(this ContractListHeader header, List<ContractListDetail> items) {
            ListModel retVal = ToListModel(header);

            if(items != null) {
                retVal.Items = items.Select(i => i.ToWebModel())
                                    .OrderBy(i => i.Position)
                                    .ToList();

                retVal.HasContractItems = (retVal.Items.Count > 0);
            }

            return retVal;
        }

        public static ListModel ToListModel(this ContractListHeader header, List<ListItemModel> items) {
            ListModel retVal = ToListModel(header);

            if(items != null) {
                retVal.Items = items.OrderBy(i => i.Position)
                                    .ToList();

                retVal.HasContractItems = (retVal.Items.Count > 0);
            }

            return retVal;
        }
        #endregion
    }
}
