using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Extensions.Lists {
    public static class ReminderHeaderExtensions {
        #region attributes
        private const string LISTNAME_REMINDER = "Reminders";
        #endregion

        #region methods
        public static ListModel ToListModel(this ReminderItemsListHeader header) {
            return new ListModel() {
                BranchId = header.BranchId,
                CustomerNumber = header.CustomerNumber,
                IsContractList = false,
                IsFavorite = false,
                IsWorksheet = false,
                IsReminder = true,
                IsMandatory = false,
                IsRecommended = false,
                IsCustomInventory = false,
                Type = ListType.Reminder,
                ListId = header.Id,
                Name = LISTNAME_REMINDER,
                ReadOnly = false
            };
        }

        public static ListModel ToListModel(this ReminderItemsListHeader header, List<ReminderItemsListDetail> items) {
            ListModel retVal = header.ToListModel();

            if (items != null) {
                retVal.Items = items.Select(i => i.ToWebModel())
                                    .OrderBy(i => i.Position)
                                    .ToList();
            }

            return retVal;
        }

        public static ListModel ToListModel(this ReminderItemsListHeader header, List<ListItemModel> items) {
            ListModel retVal = header.ToListModel();
            retVal.Items = items.OrderBy(i => i.Position)
                                .ToList();
            return retVal;
        }
        #endregion
    }
}
