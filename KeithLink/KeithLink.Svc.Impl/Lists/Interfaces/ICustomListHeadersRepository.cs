using System.Collections.Generic;
using Entree.Core.Models.Lists.CustomList;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists
{
    public interface ICustomListHeadersRepository
    {
        CustomListHeader GetCustomListHeader(long id);
        List<CustomListHeader> GetCustomListHeadersByCustomer(UserSelectedContext catalogInfo);
        long SaveCustomListHeader(CustomListHeader model);
    }
}
