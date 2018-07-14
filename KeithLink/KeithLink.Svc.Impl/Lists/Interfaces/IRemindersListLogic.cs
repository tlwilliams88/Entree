using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;
using System.Collections.Generic;

using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.ReminderItems;

namespace Entree.Core.Interface.Lists {
    public interface IRemindersListLogic : IBaseListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        void Save(UserSelectedContext catalogInfo, ReminderItemsListDetail model);

        ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);
    }
}
