using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRecentlyOrderedListHeadersRepository
    {
        RecentlyOrderedListHeader GetRecentlyOrderedHeader(string userId, UserSelectedContext catalogInfo, bool headerOnly);
    }
}
