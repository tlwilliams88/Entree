using Entree.Core.Models.Lists.ReminderItems;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists
{
    public interface IRemindersListHeadersRepository {
        ReminderItemsListHeader GetReminderItemsHeader(UserSelectedContext catalogInfo);

        long SaveReminderListHeader(ReminderItemsListHeader model);
    }
}
