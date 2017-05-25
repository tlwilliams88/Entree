﻿using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecommendedItem;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IRecommendedItemsListDetailsRepository
    {
        List<RecommendedItemsListDetail> GetRecommendedItemsDetails(long parentHeaderId);

        void AddOrUpdateRecommendedItem(string customerNumber,
            string branchId,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);

        void DeleteRecommendedItems(string userId,
            string customerNumber,
            string branchId);
    }
}
