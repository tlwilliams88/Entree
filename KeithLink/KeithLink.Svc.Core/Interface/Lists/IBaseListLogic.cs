using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IBaseListLogic
    {
        List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false);

        ListModel GetListModel(UserProfile user,
                                UserSelectedContext catalogInfo,
                                long Id);
    }
}
