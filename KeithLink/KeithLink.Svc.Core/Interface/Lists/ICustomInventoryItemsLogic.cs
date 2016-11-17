using KeithLink.Svc.Core.Models.Lists;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface ICustomInventoryItemsLogic
    {
        CustomInventoryHeaderReturnModel Delete(long id);
        CustomInventoryHeaderReturnModel DeleteRange(List<CustomInventoryItemReturnModel> items);
        CustomInventoryHeaderReturnModel DeleteRange(List<long> ids);
    }
}
