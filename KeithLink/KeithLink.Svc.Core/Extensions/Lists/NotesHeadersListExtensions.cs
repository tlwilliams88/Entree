using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Notes;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class NotesHeadersListExtensions {
        #region attributes
        private const string LISTNAME_NOTES = "Notes";
        #endregion

        #region methods
        public static ListModel ToListModel(this NotesListHeader header) {
            return new ListModel {
                BranchId = header.BranchId,
                CustomerNumber = header.CustomerNumber,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = false,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Notes,
                ListId = header.Id,
                Name = LISTNAME_NOTES,
                ReadOnly = false
            };
        }

        public static ListModel ToListModel(this NotesListHeader header, List<NotesListDetail> items) {
            ListModel retVal = ToListModel(header);
            retVal.Items = items.Select(i => i.ToWebModel())
                                .ToList();

            return retVal;
        }

        public static ListModel ToListModel(this NotesListHeader header, List<ListItemModel> items) {
            ListModel retVal = ToListModel(header);
            retVal.Items = items.ToList();

            return retVal;
        }
        #endregion
    }
}