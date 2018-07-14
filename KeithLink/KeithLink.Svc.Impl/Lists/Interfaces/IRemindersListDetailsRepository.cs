using System.Collections.Generic;

using Entree.Core.Models.Lists.ReminderItems;

namespace Entree.Core.Interface.Lists {
    public interface IRemindersListDetailsRepository {
        void DeleteReminderListDetail(long id);

        List<ReminderItemsListDetail> GetRemindersDetails(long headerId);

        long SaveReminderListDetail(ReminderItemsListDetail model);
    }
}
