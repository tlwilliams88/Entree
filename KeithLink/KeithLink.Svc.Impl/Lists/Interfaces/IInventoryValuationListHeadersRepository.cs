using System.Collections.Generic;
using Entree.Core.Models.Lists.InventoryValuationList;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists
{
    public interface IInventoryValuationListHeadersRepository
    {
        InventoryValuationListHeader GetInventoryValuationListHeader(long listId);
        List<InventoryValuationListHeader> GetInventoryValuationListHeaders(UserSelectedContext catalogInfo);
        long SaveInventoryValuationListHeader(InventoryValuationListHeader model);
    }
}
