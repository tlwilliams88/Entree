using KeithLink.Svc.Core.Enumerations.List;

using KeithLink.Svc.Core.Interface.Lists;

using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Models.Lists;
using System;
using Dapper;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class RecentlyOrderedListLogicImpl : IRecentlyOrderedListLogic
    {
        #region attributes
        private readonly IRecentlyOrderedListHeadersRepository _headersRepo;
        private readonly IRecentlyOrderedListDetailsRepository _detailsRepo;
        #endregion

        #region ctor
        public RecentlyOrderedListLogicImpl(IRecentlyOrderedListHeadersRepository headersRepo, IRecentlyOrderedListDetailsRepository detailsRepo)
        {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region methods

        public List<string> GetRecentlyOrderedItemNumbers(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<ListModel> list = ReadList(user, catalogInfo, false);

            return list[0].Items.Select(i => i.ItemNumber).ToList();
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            RecentlyOrderedListHeader header = _headersRepo.GetRecentlyOrderedHeader(user.UserId.ToString(), catalogInfo, headerOnly);

            if (header != null && headerOnly == false)
            {
                header.Items = _detailsRepo.GetRecentlyOrderedDetails(header.Id);
            }

            if (header != null)
            {
                return new List<ListModel>() { header.ToListModel(catalogInfo) };
            }
            return null;
        }

        public void AddRecentlyViewedItem(UserProfile user, UserSelectedContext catalogInfo, string itemNumber)
        {
            AddOrUpdateRecentlyOrdered(user, catalogInfo, itemNumber, false, catalogInfo.BranchId, true);
        }

        public void AddOrUpdateRecentlyOrdered(UserProfile user, 
                                UserSelectedContext catalogInfo,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                bool active)
        {
            _detailsRepo.AddOrUpdateRecentlyOrdered(user.UserId.ToString(),
                catalogInfo.CustomerId,
                catalogInfo.BranchId,
                itemNumber,
                each,
                catalogId,
                active);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            throw new NotImplementedException();
        }

        public void DeleteRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo)
        {
            _detailsRepo.DeleteRecentlyOrdered(user.UserId.ToString(), catalogInfo.CustomerId, catalogInfo.BranchId);
        }

        public void AddRecentlyOrderedItems(UserProfile user, UserSelectedContext catalogInfo, RecentNonBEKList newlist)
        {
            foreach (string itemNumber in newlist.Items.Select(i => i.ItemNumber).ToList())
            {
                // Insert newest at the start of the list while filtering out duplicates
                _detailsRepo.AddOrUpdateRecentlyOrdered(user.UserId.ToString(),
                    catalogInfo.CustomerId,
                    catalogInfo.BranchId,
                    itemNumber,
                    false,
                    newlist.Catalog,
                    true);
            }
        }

        #endregion
    }
}
