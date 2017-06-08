using KeithLink.Svc.Core.Models.Lists;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface ICustomInventoryItemsLogic
    {
        CustomInventoryHeaderReturnModel Delete(long id);
        CustomInventoryHeaderReturnModel DeleteRange(List<CustomInventoryItemReturnModel> items);
        CustomInventoryHeaderReturnModel DeleteRange(List<long> ids);
    }
}
