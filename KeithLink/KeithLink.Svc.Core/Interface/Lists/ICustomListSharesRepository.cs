using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.CustomListShares;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
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
