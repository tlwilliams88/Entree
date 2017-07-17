using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface ICustomListLogic : IBaseMultiListLogic {
        long CreateOrUpdateList(UserProfile user,
                                UserSelectedContext catalogInfo,
                                long id,
                                string name,
                                bool active);

        void DeleteList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);

        List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        ListModel ReadList(long reportId, bool headerOnly);

        void SaveItem(UserProfile user, UserSelectedContext catalogInfo, long headerId, 
                      CustomListDetail item);

        ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);
    }
}
