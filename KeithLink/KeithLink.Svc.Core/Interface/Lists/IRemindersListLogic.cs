using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRemindersListLogic : IBaseListLogic
    {
        List<string> GetRemindersNumbers(UserProfile user, UserSelectedContext catalogInfo);

        void AddOrUpdateReminder(UserSelectedContext catalogInfo,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);
    }
}
