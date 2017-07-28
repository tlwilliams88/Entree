using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IMandatoryItemsListLogic : IBaseListLogic {
        long CreateList(UserProfile user,
                        UserSelectedContext catalogInfo);

        List<string> GetMandatoryItemNumbers(UserProfile profile, UserSelectedContext catalogInfo);

        ListModel ReadList(UserProfile profile, UserSelectedContext catalogInfo, bool headerOnly);

        ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);

        void SaveDetail(UserSelectedContext catalogInfo, MandatoryItemsListDetail detail);

        void DeleteMandatoryItems(MandatoryItemsListDetail detail);
    }
}
