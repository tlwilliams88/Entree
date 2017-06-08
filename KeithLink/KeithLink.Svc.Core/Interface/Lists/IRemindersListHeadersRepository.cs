using KeithLink.Svc.Core.Models.Lists.ReminderItem;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRemindersListHeadersRepository
    {
        ReminderItemsListHeader GetReminderItemsHeader(string userId, UserSelectedContext catalogInfo, bool headerOnly);
    }
}
