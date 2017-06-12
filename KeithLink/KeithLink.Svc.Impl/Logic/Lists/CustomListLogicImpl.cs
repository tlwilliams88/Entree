﻿using System;
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

        private ListModel GetCompletedModel(CustomListHeader header, UserSelectedContext catalogInfo, bool headerOnly) {
            List<CustomListDetail> items = null;
            List<CustomListShare> shares = null;

            if(!headerOnly) {
                items = _detailsRepo.GetCustomListDetails(header.Id);
                shares = _sharesRepo.GetCustomListShares(header.Id);
            }

            return header.ToListModel(catalogInfo, shares, items);
        }

        public List<ListModel> ReadLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly) {
            List<CustomListHeader> headers = _headersRepo.GetCustomListHeadersByCustomer(catalogInfo);
            List<ListModel> list = new List<ListModel>();
            
            headers.ForEach(h => {
                list.Add(GetCompletedModel(h, catalogInfo, headerOnly));
            });

            return list;
        }

        public ListModel ReadList(long listId, UserSelectedContext catalogInfo, bool headerOnly) {
            CustomListHeader header = _headersRepo.GetCustomListHeader(listId);

            return header == null ? null : GetCompletedModel(header, catalogInfo, headerOnly);
        }

        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id)
        {
            return ReadList(Id, catalogInfo, false);
        }

        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
        {
            return ReadLists(user, catalogInfo, headerOnly);
        }
        #endregion
    }
}
