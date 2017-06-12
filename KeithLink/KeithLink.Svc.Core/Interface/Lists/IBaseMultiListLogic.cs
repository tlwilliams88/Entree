using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IBaseMultiListLogic : IBaseListLogic {
        List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false);
    }
}
