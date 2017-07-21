using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists.RecommendedItems;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class RecommendedItemsListLogicImpl : IRecommendedItemsListLogic
    {
        #region attributes
        private readonly IRecommendedItemsListDetailsRepository _detailsRepo;
        private readonly IRecommendedItemsListHeadersRepository _headersRepo;
        #endregion

        #region ctor
        public RecommendedItemsListLogicImpl(IRecommendedItemsListHeadersRepository headersRepo, IRecommendedItemsListDetailsRepository detailsRepo)
        {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region methods
        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            RecommendedItemsListHeader header = _headersRepo.GetRecommendedItemsHeaderByCustomerNumberBranch(catalogInfo);
            List<RecommendedItemsListDetail> items = null;

            if (header != null &&
                headerOnly == false)
                items = _detailsRepo.GetAllByHeader(header.Id);

            if (header != null) {
                return header.ToListModel(items);
            }
            return null;
        }

        public void SaveDetail(UserSelectedContext catalogInfo, RecommendedItemsListDetail detail)
        {
            if (detail.HeaderId == 0)
            {
                RecommendedItemsListHeader header = _headersRepo.GetRecommendedItemsHeaderByCustomerNumberBranch(catalogInfo);

                if (header == null)
                    detail.HeaderId =
                            _headersRepo.SaveRecommendedItemsHeader(new RecommendedItemsListHeader
                            {
                                BranchId = catalogInfo.BranchId,
                                CustomerNumber = catalogInfo.CustomerId
                            });
                else
                    detail.HeaderId = header.Id;
            }

            _detailsRepo.Save(detail);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            return ReadList(user, catalogInfo, false);
        }

        public long CreateList(UserSelectedContext catalogInfo)
        {
            return _headersRepo.SaveRecommendedItemsHeader(new RecommendedItemsListHeader() {
                                                                                                BranchId = catalogInfo.BranchId,
                                                                                                CustomerNumber = catalogInfo.CustomerId
                                                                                            });
        }

        public void DeleteRecommendedItems(UserProfile user, UserSelectedContext catalogInfo)
        {
            RecommendedItemsListHeader header = _headersRepo.GetRecommendedItemsHeaderByCustomerNumberBranch(catalogInfo);

            if (header != null) {
                _detailsRepo.Delete(header.Id);
            }
        }

        #endregion
    }
}
