using System.Collections.Generic;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Paging;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IListService {
        long CreateList(UserProfile user, 
                        UserSelectedContext catalogInfo, 
                        ListType type,
                        ListModel list);

        void UpdateList(UserProfile user, UserSelectedContext catalogInfo, ListModel list);

        List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo);

        RecentNonBEKList ReadRecentOrder(UserProfile user, UserSelectedContext catalogInfo, string catalog);

        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, ListType type, long Id,
                           bool includePrice = true);

        List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false);

        List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false);

        PagedListModel ReadPagedList(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                                     long Id, PagingModel paging);

        void SaveItem(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                      long headerId, ListItemModel genericItemProperties);

        List<string> ReadLabels(UserProfile user, UserSelectedContext catalogInfo);

        List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo);
    }
}
