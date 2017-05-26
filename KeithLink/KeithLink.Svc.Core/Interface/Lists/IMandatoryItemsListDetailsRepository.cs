﻿using System.Collections.Generic;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IMandatoryItemsListDetailsRepository
    {
        List<MandatoryItemsListDetail> GetMandatoryItemsDetails(long parentHeaderId);

        void AddOrUpdateMandatoryItem(string customerNumber,
            string branchId,
            string itemNumber,
            bool each,
            string catalogId,
            bool active);

        void DeleteMandatoryItems(string userId,
            string customerNumber,
            string branchId);
    }
}
