using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IMandatoryItemsListLogic : IBaseListLogic
    {
        List<string> GetMandatoryItemNumbers(UserProfile user, UserSelectedContext catalogInfo);

        void AddOrUpdateMandatoryItem(UserSelectedContext catalogInfo,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);
    }
}
