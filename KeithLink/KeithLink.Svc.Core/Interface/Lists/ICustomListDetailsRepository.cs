using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface ICustomListDetailsRepository
    {
        List<CustomListDetail> GetCustomListDetails(long parentHeaderId);

        void AddOrUpdateCustomListItem(
            long parentCustomListHeaderId,
            string itemNumber,
            bool each,
            decimal par,
            string catalogId,
            bool active);
    }
}
