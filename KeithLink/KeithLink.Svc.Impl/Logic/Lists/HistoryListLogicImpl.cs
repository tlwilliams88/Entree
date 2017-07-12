using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.History;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Lists;

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class HistoryListLogicImpl : IHistoryListLogic
    {
        #region attributes
        private readonly IHistoryListDetailRepository _detailRepo;
        private readonly IHistoryListHeaderRepository _headerRepo;
        private readonly IEventLogRepository          _log;
        #endregion

        #region ctor
        public HistoryListLogicImpl(IHistoryListDetailRepository detailRepository, IHistoryListHeaderRepository headerRepository, IEventLogRepository log) {
            _detailRepo = detailRepository;
            _headerRepo = headerRepository;
            _log        = log;
        }
        #endregion

        #region methods
        public ListModel GetListModel(UserProfile user, UserSelectedContext catalogInfo, long Id) {
            HistoryListHeader header = _headerRepo.GetHistoryListHeader(catalogInfo);

            if (header == null) {
                return null;
            } else {
                List<HistoryListDetail> items = _detailRepo.GetAllHistoryDetails(header.Id);

                return header.ToListModel(items);
            }
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
