using System;
using System.Collections.Generic;
using System.Linq;

using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.RecentlyOrdered;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class RecentlyOrderedListLogicImpl : IRecentlyOrderedListLogic {
        #region ctor
        public RecentlyOrderedListLogicImpl(IRecentlyOrderedListHeadersRepository headersRepo, IRecentlyOrderedListDetailsRepository detailsRepo) {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region attributes
        private readonly IRecentlyOrderedListHeadersRepository _headersRepo;
        private readonly IRecentlyOrderedListDetailsRepository _detailsRepo;
        #endregion

        #region methods

        public void Save(UserProfile user, UserSelectedContext catalogInfo, string itemNumber, bool each, string catalogId, bool active) {
            throw new NotImplementedException();
        }

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly) {
            RecentlyOrderedListHeader returnValue = _headersRepo.GetRecentlyOrderedHeader(user.UserId, catalogInfo);
            List<RecentlyOrderedListDetail> details = null;

            if (returnValue != null &&
                headerOnly == false)
                details = _detailsRepo.GetRecentlyOrderedjetails(returnValue.Id);

            return returnValue.ToListModel(details);
        }

        public void PostRecentOrder(UserProfile user,
                                    UserSelectedContext catalogInfo,
                                    RecentNonBEKList list) {
            long headerid = 0;

            foreach (RecentNonBEKItem order in list.Items) {
                headerid = Save(user, catalogInfo, order.ItemNumber, false, order.CatalogId);
            }

            DeleteOldRecentlyOrdered(user, catalogInfo, headerid);
        }

        public long Save(UserProfile user,
                         UserSelectedContext catalogInfo,
                         string itemNumber,
                         bool each,
                         string catalogId) {
            RecentlyOrderedListHeader header = _headersRepo.GetRecentlyOrderedHeader(user.UserId, catalogInfo);
            RecentlyOrderedListDetail detail = new RecentlyOrderedListDetail {
                                                                                 CatalogId = catalogId,
                                                                                 ItemNumber = itemNumber,
                                                                                 Each = each
                                                                             };

            if (header == null) {
                // Create the header
                header = new RecentlyOrderedListHeader {
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
                              UserSelectedContext catalogInfo)
        {
            RecentlyOrderedListHeader header = _headersRepo.GetRecentlyOrderedHeader(user.UserId, catalogInfo);

            _detailsRepo.DeleteOldRecentlyOrdered(header.Id, 0);
        }

        public void DeleteRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo, RecentlyOrderedListDetail details) {
            _detailsRepo.DeleteRecentlyOrdered(details);
        }

        public void DeleteOldRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo, long headerId) {
            _detailsRepo.DeleteOldRecentlyOrdered(headerId);
        }
        #endregion

    }
}