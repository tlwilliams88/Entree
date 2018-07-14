using System.Collections.Generic;
using Entree.Core.Models.Lists.InventoryValuationList;

namespace Entree.Core.Interface.Lists
{
    public interface IInventoryValuationListDetailsRepository
    {
        List<InventoryValuationListDetail> GetInventoryValuationDetails(long headerId);

        long SaveInventoryValuationDetail(InventoryValuationListDetail model);
    }
}
