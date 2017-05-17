using System;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IHistoryListRepository
    {
        List<ListModel> ReadListForCustomer(UserSelectedContext catalogInfo, bool headerOnly);
    }
}
