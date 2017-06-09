using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface ICustomListDetailsRepository
    {
        List<CustomListDetail> GetCustomListDetails(long headerId);

        void SaveCustomListDetail(CustomListDetail model);
    }
}
