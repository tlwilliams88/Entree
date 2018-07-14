using Entree.Core.Models.Lists;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists
{
    public interface IContractListLogic : IBaseListLogic {
        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);
    }
}
