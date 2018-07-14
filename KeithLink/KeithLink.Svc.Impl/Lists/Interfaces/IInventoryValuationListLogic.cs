using System.Collections.Generic;

using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.InventoryValuationList;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
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
