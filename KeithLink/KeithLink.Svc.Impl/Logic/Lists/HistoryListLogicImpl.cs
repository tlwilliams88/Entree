using System;
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
        #endregion
    }
}
