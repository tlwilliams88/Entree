using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.ReminderItems;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRemindersListDetailsRepository {
        void DeleteReminderListDetail(long id);

        List<ReminderItemsListDetail> GetRemindersDetails(long headerId);

        void SaveReminderListDetail(ReminderItemsListDetail model);
    }
}
