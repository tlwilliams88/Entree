using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface ICustomListHeadersRepository
    {
        CustomListHeader GetCustomListHeader(long id);
        List<CustomListHeader> GetCustomListHeadersByCustomer(UserSelectedContext catalogInfo);
        long SaveCustomListHeader(CustomListHeader model);
    }
}
