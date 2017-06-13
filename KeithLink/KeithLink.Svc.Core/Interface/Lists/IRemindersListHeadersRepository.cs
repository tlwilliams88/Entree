using KeithLink.Svc.Core.Models.Lists.ReminderItems;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRemindersListHeadersRepository {
        ReminderItemsListHeader GetReminderItemsHeader(UserSelectedContext catalogInfo);

        long SaveReminderListHeader(ReminderItemsListHeader model);
    }
}
