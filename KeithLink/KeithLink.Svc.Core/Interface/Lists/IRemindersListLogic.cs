using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.ReminderItems;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRemindersListLogic : IBaseListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        void Save(UserSelectedContext catalogInfo, ReminderItemsListDetail model);

        ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);
    }
}
