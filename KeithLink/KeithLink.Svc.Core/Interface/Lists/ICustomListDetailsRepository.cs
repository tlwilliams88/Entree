using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface ICustomListDetailsRepository {
        void DeleteCustomListDetails(long id);

        List<CustomListDetail> GetCustomListDetails(long headerId);

        long SaveCustomListDetail(CustomListDetail model);
    }
}
