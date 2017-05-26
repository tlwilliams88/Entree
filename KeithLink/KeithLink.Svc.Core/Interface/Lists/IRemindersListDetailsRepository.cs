using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.ReminderItem;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRemindersListDetailsRepository
    {
        List<ReminderItemsListDetail> GetRemindersDetails(long parentHeaderId);

        void AddOrUpdateReminder(string customerNumber,
            string branchId,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);

        void DeleteReminders(string userId,
            string customerNumber,
            string branchId);
    }
}
