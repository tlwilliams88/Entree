using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IInventoryValuationListHeadersRepository
    {
        InventoryValuationListHeader GetInventoryValuationListHeader(long listId);
        List<InventoryValuationListHeader> GetInventoryValuationListHeaders(UserSelectedContext catalogInfo);
        long SaveInventoryValudationListHeader(InventoryValuationListHeader model);
    }
}
