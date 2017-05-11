using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IListService
    {
        List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false);
    }
}
