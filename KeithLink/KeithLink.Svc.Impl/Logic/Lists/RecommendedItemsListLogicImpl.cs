using System.Collections.Generic;
using System.Linq;
using KeithLink.Svc.Core.Extensions;
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
        public List<string> GetRecommendedItemNumbers(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<ListModel> list = ReadList(user, catalogInfo, false);

            return list[0].Items.Select(i => i.ItemNumber).ToList();
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            RecommendedItemsListHeader header = _headersRepo.GetRecommendedItemsHeader(user.UserId.ToString(), catalogInfo, headerOnly);

            if (header != null && headerOnly == false)
            {
                //header.Items = _detailsRepo.GetRecommendedItemsDetails(header.Id);
            }

            if (header != null)
            {
                return new List<ListModel>() { header.ToListModel(catalogInfo) };
            }
            return null;
        }

        public void AddOrUpdateRecommendedItem(UserSelectedContext catalogInfo,
                                string itemNumber,
                                bool each,
                                string catalogId,
                                bool active)
        {
            _detailsRepo.AddOrUpdateRecommendedItem(catalogInfo.CustomerId,
                catalogInfo.BranchId,
                itemNumber,
                each,
                catalogId,
                active);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            return ReadList(user, catalogInfo, false)[0];
        }

        public void DeleteRecentlyOrdered(UserProfile user, UserSelectedContext catalogInfo)
        {
            _detailsRepo.DeleteRecommendedItems(user.UserId.ToString(), catalogInfo.CustomerId, catalogInfo.BranchId);
        }

        #endregion
    }
}
