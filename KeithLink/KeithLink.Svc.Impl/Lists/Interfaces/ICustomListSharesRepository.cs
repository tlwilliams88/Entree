using System.Collections.Generic;

using Entree.Core.Models.Lists.CustomListShares;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists
{
    public interface ICustomListSharesRepository
    {
        void DeleteCustomListShares(long id);
        CustomListShare GetCustomListShare(long id);
        List<CustomListShare> GetCustomListSharesByCustomer(UserSelectedContext catalogInfo);
        List<CustomListShare> GetCustomListSharesByHeaderId(long parentId);
        long SaveCustomListShare(CustomListShare model);
    }
}
