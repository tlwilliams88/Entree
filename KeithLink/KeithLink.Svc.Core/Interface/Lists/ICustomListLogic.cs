using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface ICustomListLogic : IBaseMultiListLogic {
        List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        ListModel ReadList(long reportId, UserSelectedContext catalogInfo, bool headerOnly);

        void SaveItem(UserProfile user, UserSelectedContext catalogInfo, long headerId, 
                      CustomListDetail item);
    }
}
