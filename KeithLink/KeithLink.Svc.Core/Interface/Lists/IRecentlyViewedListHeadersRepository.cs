using System;

using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IRecentlyViewedListHeadersRepository {
        RecentlyViewedListHeader GetRecentlyViewedHeader(Guid userId, UserSelectedContext catalogInfo);
        long Save(RecentlyViewedListHeader header, Guid userId);
    }
}