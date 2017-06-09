using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IHistoryListRepository
    {
        List<ListModel> ReadListForCustomer(UserSelectedContext catalogInfo, bool headerOnly);
    }
}
