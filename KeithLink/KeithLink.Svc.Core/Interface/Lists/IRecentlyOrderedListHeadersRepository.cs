using System;

using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyOrderedListHeadersRepository {
        RecentlyOrderedListHeader GetRecentlyOrderedHeader(Guid userId, UserSelectedContext catalogInfo);
        long Save(RecentlyOrderedListHeader header, Guid userId);
    }
}