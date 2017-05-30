using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.CloudFront.Model;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class InventoryValuationListLogicImpl : IInventoryValuationListLogic
    {
        #region attributes
        private readonly IInventoryValuationListDetailsRepository _detailsRepo;
        private readonly IInventoryValuationListHeadersRepository _headersRepo;
        #endregion

        #region ctor
        public InventoryValuationListLogicImpl(IInventoryValuationListHeadersRepository headersRepo, IInventoryValuationListDetailsRepository detailsRepo)
        {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
        }
        #endregion

        #region methods
        public List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            List<InventoryValuationListHeader> headers = _headersRepo.GetInventoryValuationListHeaders(catalogInfo);
            List<ListModel> list = new List<ListModel>();

            if (headers != null && headerOnly == false)
            {
                foreach (var header in headers)
                {
                    header.Items = _detailsRepo.GetInventoryValuationDetails(header.Id);
                    list.Add(header.ToListModel(catalogInfo));
                }
                return list;
            }
            return null;
        }

        public List<ListModel> ReadList(long reportId, UserSelectedContext catalogInfo, bool headerOnly)
        {
            InventoryValuationListHeader header = _headersRepo.GetInventoryValuationListHeader(reportId);

            if (header != null && headerOnly == false)
            {
               header.Items = _detailsRepo.GetInventoryValuationDetails(header.Id);
            }
            if (header != null)
            {
                return new List<ListModel>() { header.ToListModel(catalogInfo)};
            }
            return null;
        }

        public void AddOrUpdateInventoryValuationItem(UserSelectedContext catalogInfo,
                                long listId,
                                string listName,
                                string itemNumber,
                                bool each,
                                decimal quantity,
                                string catalogId,
                                bool active)
        {
            _detailsRepo.AddOrUpdateRecommendedItem(catalogInfo.CustomerId,
                catalogInfo.BranchId,
                listId,
                listName,
                itemNumber,
                each,
                quantity,
                catalogId,
                active);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            throw new NotImplementedException();
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
