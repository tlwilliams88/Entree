using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface ICustomListHeadersRepository
    {
        InventoryValuationListHeader GetCustomListHeader(long reportId);
        List<InventoryValuationListHeader> GetCustomListHeaders(UserSelectedContext catalogInfo);
    }
}
