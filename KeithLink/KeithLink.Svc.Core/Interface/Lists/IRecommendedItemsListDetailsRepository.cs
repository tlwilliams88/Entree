using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists.RecommendedItem;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRecommendedItemsListDetailsRepository
    {
        List<RecommendedItemsListDetail> GetAllByHeader(long parentHeaderId);

        void Save(RecommendedItemsListDetail model);

        void Delete(long id);
    }
}
