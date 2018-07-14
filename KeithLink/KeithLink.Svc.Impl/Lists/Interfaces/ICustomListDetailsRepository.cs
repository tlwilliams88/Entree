using System.Collections.Generic;
using Entree.Core.Models.Lists.CustomList;

namespace Entree.Core.Interface.Lists
{
    public interface ICustomListDetailsRepository {
        List<CustomListDetail> GetCustomListDetails(long headerId);

        long SaveCustomListDetail(CustomListDetail model);
    }
}
