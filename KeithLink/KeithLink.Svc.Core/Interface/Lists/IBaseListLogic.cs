using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IBaseListLogic {
        ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long id);
    }
}
