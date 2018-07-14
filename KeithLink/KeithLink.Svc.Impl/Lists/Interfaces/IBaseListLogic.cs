using Entree.Core.Models.Lists;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface IBaseListLogic {
        ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long id);
    }
}
