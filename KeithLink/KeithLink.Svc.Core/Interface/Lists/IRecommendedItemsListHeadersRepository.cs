using KeithLink.Svc.Core.Models.Lists.RecommendedItems;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRecommendedItemsListHeadersRepository
    {
        RecommendedItemsListHeader GetRecommendedItemsHeader(string userId, UserSelectedContext catalogInfo, bool headerOnly);
    }
}
