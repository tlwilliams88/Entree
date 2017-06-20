using KeithLink.Svc.Core.Models.Lists.RecommendedItems;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRecommendedItemsListHeadersRepository
    {
        RecommendedItemsListHeader GetRecommendedItemsHeaderByCustomerNumberBranch(UserSelectedContext catalogInfo);

        long SaveRecommendedItemsHeader(RecommendedItemsListHeader model);
    }
}
