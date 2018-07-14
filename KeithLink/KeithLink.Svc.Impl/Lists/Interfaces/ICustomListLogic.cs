using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;
using System.Collections.Generic;
using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.CustomList;

namespace Entree.Core.Interface.Lists {
    public interface ICustomListLogic : IBaseMultiListLogic {
        long CreateOrUpdateList(UserProfile user,
                                UserSelectedContext catalogInfo,
                                long id,
                                string name,
                                bool active);

        void DeleteList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);

        List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        ListModel ReadList(long reportId, UserSelectedContext catalogInfo, bool headerOnly);

        void SaveItem(CustomListDetail item);

        ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);
    }
}
