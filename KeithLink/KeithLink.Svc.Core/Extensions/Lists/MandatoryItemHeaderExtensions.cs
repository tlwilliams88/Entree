using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class MandatoryItemHeaderExtensions {
        #region attributes
        private const string LISTNAME_MANDATORY = "Mandatory";
        #endregion

        #region methods
        public static ListModel ToListModel(this MandatoryItemsListHeader header) {
            return new ListModel() {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = true,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Mandatory,
                ListId = header.Id,
                Name = LISTNAME_MANDATORY,
                ReadOnly = true,
                Items = null
            };
        }

        public static ListModel ToListModel(this MandatoryItemsListHeader header, List<MandatoryItemsListDetail> items) {
            ListModel retVal = null;
            if (header != null)
            {
                retVal = ToListModel(header);
            }

            if (items != null) {
                retVal.Items = items.Select(i => i.ToWebModel())
                                    .OrderBy(i => i.Position)
                                    .ToList();
            }

            return retVal;
        }

        public static ListModel ToListModel(this MandatoryItemsListHeader header, List<ListItemModel> items) {
            ListModel retVal = ToListModel(header);
            retVal.Items = items.OrderBy(i => i.Position)
                                .ToList();

            return retVal;
        }
        #endregion
    }
}
