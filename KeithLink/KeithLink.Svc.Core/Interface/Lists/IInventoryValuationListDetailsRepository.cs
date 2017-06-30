using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IInventoryValuationListDetailsRepository
    {
        List<InventoryValuationListDetail> GetInventoryValuationDetails(long headerId);

        long SaveInventoryValudationDetail(InventoryValuationListDetail model);
    }
}
