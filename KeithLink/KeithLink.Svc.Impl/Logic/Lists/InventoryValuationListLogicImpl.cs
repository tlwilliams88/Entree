using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.CloudFront.Model;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists.InventoryValuationList;

namespace KeithLink.Svc.Impl.Logic.Lists {
    public class InventoryValuationListLogicImpl : IInventoryValuationListLogic {
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
        private ListModel GetCompletedLists(InventoryValuationListHeader header, bool headerOnly) {
            List<InventoryValuationListDetail> items = null;

            if (!headerOnly) {
                items = _detailsRepo.GetInventoryValuationDetails(header.Id);
            }

            return header.ToListModel(items);
        }

        public List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly) {
            List<InventoryValuationListHeader> headers = _headersRepo.GetInventoryValuationListHeaders(catalogInfo);
            List<ListModel> list = new List<ListModel>();

            headers.ForEach(h => {
                list.Add(GetCompletedLists(h, headerOnly));
            });

            return list;
        }

        public ListModel ReadList(long reportId, UserSelectedContext catalogInfo, bool headerOnly) {
            InventoryValuationListHeader header = _headersRepo.GetInventoryValuationListHeader(reportId);

            return header == null ? null : GetCompletedLists(header, headerOnly);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long id) {
            return ReadList(id, catalogInfo, false);
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false) {
            return ReadLists(user, catalogInfo, headerOnly);
        }

        public void SaveItem(UserProfile user, UserSelectedContext catalogInfo, long headerId,
                             InventoryValuationListDetail item) {
            // try to find the parent header id if it is not in the model
            if(item.HeaderId  == 0) {
                const string HEADER_MISSING_TEXT = "No header id was set";
                throw new ArgumentException(HEADER_MISSING_TEXT, nameof(item.HeaderId));
            }

            _detailsRepo.SaveInventoryValudationDetail(item);
        }

        #endregion
    }
}
