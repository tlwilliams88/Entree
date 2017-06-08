using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IListService
    {
        List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo);

        RecentNonBEKList ReadRecentOrder(UserProfile user, UserSelectedContext catalogInfo, string catalog);

        ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, ListType type, long Id,
            bool includePrice = true);

        List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false);

        List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false);

        PagedListModel ReadPagedList(UserProfile user,
                                     UserSelectedContext catalogInfo,
                                     ListType type,
                                     long Id,
                                     Core.Models.Paging.PagingModel paging);

        long AddOrUpdateItem(UserProfile user,
                             UserSelectedContext catalogInfo,
                             ListType type,
                             dynamic genericItemProperties);
    }
}
