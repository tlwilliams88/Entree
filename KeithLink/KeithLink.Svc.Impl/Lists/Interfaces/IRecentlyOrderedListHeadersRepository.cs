using System;

using Entree.Core.Models.Lists.RecentlyOrdered;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface IRecentlyOrderedListHeadersRepository {
        RecentlyOrderedListHeader GetRecentlyOrderedHeader(Guid userId, UserSelectedContext catalogInfo);
        long Save(RecentlyOrderedListHeader header);
    }
}