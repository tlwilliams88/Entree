using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists.CustomListShares;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface ICustomListSharesRepository
    {
        List<CustomListShare> GetCustomListShares(UserSelectedContext catalogInfo);
        List<CustomListShare> GetCustomListShares(long parentId);
    }
}
