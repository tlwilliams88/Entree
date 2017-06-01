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
using KeithLink.Svc.Core.Models.Lists.CustomList;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class CustomListLogicImpl : ICustomListLogic
    {
        #region attributes
        private readonly ICustomListDetailsRepository _detailsRepo;
        private readonly ICustomListHeadersRepository _headersRepo;
        private readonly ICustomListSharesRepository _customListSharesRepository;
        #endregion

        #region ctor
        public CustomListLogicImpl(ICustomListSharesRepository customListSharesRepository, ICustomListHeadersRepository headersRepo, ICustomListDetailsRepository detailsRepo)
        {
            _headersRepo = headersRepo;
            _detailsRepo = detailsRepo;
            _customListSharesRepository = customListSharesRepository;
        }
        #endregion

        #region methods
        public List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            List<CustomListHeader> headers = _headersRepo.GetCustomListHeaders(catalogInfo);
            List<ListModel> list = new List<ListModel>();
            
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    if (headerOnly == false)
                    {
                        header.Items = _detailsRepo.GetCustomListDetails(header.Id);
                    }
                    if (header != null)
                    {
                        var sharedwithme = _customListSharesRepository.GetCustomListShares(catalogInfo);

                        list.Add(header.ToListModel(catalogInfo));
                    }
                }
                return list;
            }
            return null;
        }

        public List<ListModel> ReadList(long listId, UserSelectedContext catalogInfo, bool headerOnly)
        {
            CustomListHeader header = _headersRepo.GetCustomListHeader(listId);

            if (header != null && headerOnly == false)
            {
               header.Items = _detailsRepo.GetCustomListDetails(header.Id);
            }
            if (header != null)
            {
                return new List<ListModel>() { header.ToListModel(catalogInfo)};
            }
            return null;
        }

        public void AddOrUpdateCustomListItem(long listId,
                                string itemNumber,
                                bool each,
                                decimal par,
                                string catalogId,
                                bool active)
        {
            _detailsRepo.AddOrUpdateCustomListItem(listId,
                itemNumber,
                each,
                par,
                catalogId,
                active);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            return ReadList(Id, catalogInfo, false)[0];
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
        {
            return ReadLists(user, catalogInfo, headerOnly);
        }
        #endregion
    }
}
