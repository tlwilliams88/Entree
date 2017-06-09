using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.History;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class HistoryHeaderExtensions {
        #region attributes
        private const string LISTNAME_HISTORY = "History";
        #endregion

        #region methods

        public static ListModel ToListModel(this HistoryListHeader header)
        {
            return new ListModel()
            {
                BranchId = header.BranchId,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = true,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Worksheet,
                ListId = header.Id,
                Name = LISTNAME_HISTORY,
                ReadOnly = true,
                Items = null 
            };
        }

        public static ListModel ToListModel(this HistoryListHeader header, List<HistoryListDetail> items) {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.Select(i => i.ToWebModel())
                                .OrderBy(i => i.Position)
                                .ToList();

            return retVal;
        }

        public static ListModel ToListModel(this HistoryListHeader header, List<ListItemModel> items) {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.OrderBy(i => i.Position)
                                .ToList();

            return retVal;
        }
        #endregion
    }
}
