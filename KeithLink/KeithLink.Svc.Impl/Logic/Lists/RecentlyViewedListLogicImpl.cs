using System;
using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyViewed;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class RecentlyViewedListLogicImpl : IRecentlyViewedListLogic {
        #region ctor
        public RecentlyViewedListLogicImpl(IRecentlyViewedListHeadersRepository headersRepo, IRecentlyViewedListDetailsRepository detailsRepo) {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region attributes
        private readonly IRecentlyViewedListHeadersRepository _headersRepo;
        private readonly IRecentlyViewedListDetailsRepository _detailsRepo;
        #endregion

        #region methods

        public void Save(UserProfile user, UserSelectedContext catalogInfo, string itemNumber, bool each, string catalogId, bool active) {
            throw new NotImplementedException();
        }

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly) {
            RecentlyViewedListHeader returnValue = _headersRepo.GetRecentlyViewedHeader(user.UserId, catalogInfo);
            List<RecentlyViewedListDetail> details = null;

            if (returnValue != null &&
                headerOnly == false)
                details = _detailsRepo.GetRecentlyViewedDetails(returnValue.Id);

            return returnValue.ToListModel(details);
        }

        public void PostRecentOrder(UserProfile user,
                                    UserSelectedContext catalogInfo,
                                    RecentNonBEKList list) {
            long headerid = 0;

            foreach (RecentNonBEKItem order in list.Items) {
                headerid = Save(user, catalogInfo, order.ItemNumber, false, order.CatalogId);
            }

            DeleteOldRecentlyViewed(user, catalogInfo, headerid);
        }

        public long Save(UserProfile user,
                         UserSelectedContext catalogInfo,
                         string itemNumber,
                         bool each,
                         string catalogId) {
            RecentlyViewedListHeader header = _headersRepo.GetRecentlyViewedHeader(user.UserId, catalogInfo);
            RecentlyViewedListDetail detail = new RecentlyViewedListDetail {
                                                                                 CatalogId = catalogId,
                                                                                 ItemNumber = itemNumber,
                                                                                 Each = each
                                                                             };

            if (header == null) {
                // Create the header
                header = new RecentlyViewedListHeader {
                                                           Id = 0,
                                                           BranchId = catalogInfo.BranchId,
                                                           CustomerNumber = catalogInfo.CustomerId
                                                       };

                header.Id = _headersRepo.Save(header, user.UserId);
            }

            detail.HeaderId = header.Id;
            detail.CatalogId = catalogInfo.BranchId;

            _detailsRepo.Save(detail);

            return header.Id;
        }

        public void DeleteAll(UserProfile user,
                              UserSelectedContext catalogInfo) {
            RecentlyViewedListHeader header = _headersRepo.GetRecentlyViewedHeader(user.UserId, catalogInfo);

            int daysToKeep = 0;
            _detailsRepo.DeleteOldRecentlyViewed(header.Id, daysToKeep);
        }

        public void DeleteRecentlyViewed(UserProfile user, UserSelectedContext catalogInfo, RecentlyViewedListDetail details) {
            _detailsRepo.DeleteRecentlyViewed(details);
        }

        public void DeleteOldRecentlyViewed(UserProfile user, UserSelectedContext catalogInfo, long headerId) {
            _detailsRepo.DeleteOldRecentlyViewed(headerId);
        }
        #endregion

    }
}