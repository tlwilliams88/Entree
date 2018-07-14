using System;

using Entree.Core.Models.Lists.RecentlyViewed;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface IRecentlyViewedListHeadersRepository {
        RecentlyViewedListHeader GetRecentlyViewedHeader(Guid userId, UserSelectedContext catalogInfo);
        long Save(RecentlyViewedListHeader header);
    }
}