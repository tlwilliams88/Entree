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
    public interface IBaseListLogic
    {
        List<ListModel> ReadList(UserSelectedContext catalogInfo, bool headerOnly = false);

        ListModel GetListModel(UserProfile user,
                                UserSelectedContext catalogInfo,
                                long Id);
    }
}
