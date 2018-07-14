﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entree.Core.Interface.Lists;
using Entree.Core.Models.Lists;
using Entree.Core.Models.Lists.History;
using Entree.Core.Models.Profile;
using Entree.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Common.Core.Interfaces.Logging;
using Entree.Core.Enumerations.List;
using Entree.Core.Extensions;
using Entree.Core.Extensions.Lists;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class HistoryListLogicImpl : IHistoryListLogic
    {
        #region attributes
        private readonly IHistoryListDetailRepository _detailRepo;
        private readonly IHistoryListHeaderRepository _headerRepo;
        #endregion

        #region ctor
        public HistoryListLogicImpl(IHistoryListDetailRepository detailRepository, IHistoryListHeaderRepository headerRepository) {
            _detailRepo = detailRepository;
            _headerRepo = headerRepository;
        }
        #endregion

        #region methods
        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id) {
            ListModel list = null;

            HistoryListHeader header = _headerRepo.GetHistoryListHeader(catalogInfo);

            if (header == null) {
                list = null;
            } else {
                List<HistoryListDetail> items = _detailRepo.GetAllHistoryDetails(header.Id);

                list = header.ToListModel(items);
            }
            return list;
        }

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly)
        {
            ListModel list = null;

            HistoryListHeader header = _headerRepo.GetHistoryListHeader(catalogInfo);

            if (header == null)
            {
                list = null;
            }
            else
            {
                if (headerOnly) {
                    list = header.ToListModel();
                } else {
                    List<HistoryListDetail> items = _detailRepo.GetAllHistoryDetails(header.Id);

                    list = header.ToListModel(items);
                }
            }
            return list;
        }

        public List<InHistoryReturnModel> ItemsInHistoryList(UserSelectedContext catalogInfo, List<string> itemNumbers)
        {
            var returnModel = new BlockingCollection<InHistoryReturnModel>();

            HistoryListHeader list = _headerRepo.GetHistoryListHeader(catalogInfo);

            if (list == null)
                return itemNumbers.Select(i => new InHistoryReturnModel() { ItemNumber = i, InHistory = false })
                                  .ToList();
            else
            {
                List<HistoryListDetail> items = _detailRepo.GetAllHistoryDetails(list.Id);
                Parallel.ForEach(itemNumbers, item => {
                    returnModel.Add(new InHistoryReturnModel() { InHistory = items.Where(i => i.ItemNumber.Equals(item)).Any(), ItemNumber = item });
                });

                return returnModel.ToList();
            }
        }
        #endregion
    }
}
