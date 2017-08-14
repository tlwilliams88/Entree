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
using KeithLink.Svc.Core.Models.Lists.CustomList;
using KeithLink.Svc.Core.Models.Lists.CustomListShares;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class CustomListLogicImpl : ICustomListLogic
    {
        #region attributes
        private readonly ICustomListDetailsRepository _detailsRepo;
        private readonly ICustomListHeadersRepository _headersRepo;
        private readonly ICustomListSharesRepository _sharesRepo;
        #endregion

        #region ctor
        public CustomListLogicImpl(ICustomListSharesRepository customListSharesRepository, ICustomListHeadersRepository headersRepo, ICustomListDetailsRepository detailsRepo)
        {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
            _sharesRepo = customListSharesRepository;
        }
        #endregion

        #region methods

        private ListModel GetCompletedModel(CustomListHeader header, bool headerOnly) {
            List<CustomListDetail> items = null;
            List<CustomListShare> shares = null;

            if(!headerOnly) {
                items = _detailsRepo.GetCustomListDetails(header.Id);
                shares = _sharesRepo.GetCustomListSharesByHeaderId(header.Id);
            }

            UserSelectedContext catalogInfo = new UserSelectedContext();

            if(header != null) {
                catalogInfo.BranchId = header.BranchId;
                catalogInfo.CustomerId = header.CustomerNumber;
            }

            return header.ToListModel(catalogInfo, shares, items);
        }

        public List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly) {
            List<CustomListHeader> headers = _headersRepo.GetCustomListHeadersByCustomer(catalogInfo);
            var shares = _sharesRepo.GetCustomListSharesByCustomer(catalogInfo);
            if (shares != null) {
                if (headers == null) {
                    headers = new List<CustomListHeader>();
                }
                foreach (var share in shares) {
                    headers.Add(_headersRepo.GetCustomListHeader(share.HeaderId));
                }
            }
            List<ListModel> list = new List<ListModel>();

            if(headers != null) {
                headers.ForEach(h => {
                    list.Add(GetCompletedModel(h, headerOnly));
                });
            }

            return list;
        }

        public ListModel ReadList(long listId, bool headerOnly) {
            CustomListHeader header = _headersRepo.GetCustomListHeader(listId);

            return header == null ? null : GetCompletedModel(header, headerOnly);
        }

        public ListModel SaveList(UserProfile user, UserSelectedContext catalogInfo, ListModel list)
        {
            CreateOrUpdateList(user, catalogInfo, list.ListId, list.Name, true);
            foreach (var item in list.Items)
            {
                CustomListDetail detail = item.ToCustomListDetail(list.ListId);
                detail.Active = !item.IsDelete;
                SaveItem(detail);
            }

            return ReadList(list.ListId, false);
        }

        public void DeleteList(UserProfile user, UserSelectedContext catalogInfo, ListModel list)
        {
            CreateOrUpdateList(user, catalogInfo, list.ListId, list.Name, false);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id) {
            return ReadList(Id, false);
        }

        public void SaveItem(CustomListDetail item) {
            // try to find the parent header id if it is not in the model
            if(item.HeaderId == 0) {
                const string HEADER_MISSING_TEXT = "No header id was set";
                throw new ArgumentException(HEADER_MISSING_TEXT, nameof(item.HeaderId));
            }

            _detailsRepo.SaveCustomListDetail(item);
        }

        public long CreateOrUpdateList(UserProfile user, UserSelectedContext catalogInfo, long id,
                                       string name, bool active) {
            return _headersRepo.SaveCustomListHeader(new CustomListHeader() {
                                                                                Id = id,
                                                                                UserId = user.UserId,
                                                                                CustomerNumber = catalogInfo.CustomerId,
                                                                                BranchId = catalogInfo.BranchId,
                                                                                Name = name,
                                                                                Active = active
                                                                            });
        }
        #endregion
    }
}
