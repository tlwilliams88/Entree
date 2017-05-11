using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class HistoryListLogicImpl : IHistoryListLogic
    {
        public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
        {
            throw new NotImplementedException();
        }
    }
}
