using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IInventoryValuationListLogic : IBaseListLogic
    {

        List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly);

        List<ListModel> ReadList(long reportId, UserSelectedContext catalogInfo, bool headerOnly);

        void AddOrUpdateInventoryValuationItem(UserSelectedContext catalogInfo,
                                long listId,
                                string listName,
                                string itemNumber,
                                bool each,
                                decimal quantity,
                                string catalogId,
                                bool active);
    }
}
