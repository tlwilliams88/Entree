using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IInventoryValuationListLogic : IBaseMultiListLogic {
        List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        ListModel ReadList(long reportId, UserSelectedContext catalogInfo, bool headerOnly);

        long CreateOrUpdateList(UserProfile user,
                                UserSelectedContext catalogInfo,
                                long id,
                                string name,
                                bool active);

        void SaveItem(UserProfile user, UserSelectedContext catalogInfo, long headerId,
                      InventoryValuationListDetail item);

        ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);
    }
}
