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

namespace KeithLink.Svc.Impl.Logic.Lists
{
    public class HistoryListLogicImpl : IHistoryListLogic
    {
        #region attributes
        private readonly IHistoryListRepository _historyListrepo;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        public HistoryListLogicImpl(IHistoryListRepository historyListrepo, IEventLogRepository log)
        {
            _historyListrepo = historyListrepo;
            _log = log;
        }
        #endregion
        public List<ListModel> ReadList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
        {
            return _historyListrepo.ReadListForCustomer(catalogInfo, headerOnly);
        }

        public ListModel GetListModel (UserProfile user,
                                       UserSelectedContext catalogInfo,
                                       long Id)
        {
            System.Diagnostics.Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(gettiming: false);
            ListModel returnList = null;
            //ListModel cachedList = _cache.GetItem<ListModel>(CACHE_GROUPNAME,
            //                                     CACHE_PREFIX,
            //                                     CACHE_NAME,
            //                                     string.Format("UserList_{0}", Id));
            //stopWatch.Read(_log, "GetListModel - GetItem");

            //if (cachedList != null)
            //{
            //    ListModel cachedReturnList = cachedList.ShallowCopy();
            //    stopWatch.Read(_log, "GetListModel - ShallowCopy");

            //    RefreshSharingProps(catalogInfo, Id, cachedReturnList);
            //    stopWatch.Read(_log, "GetListModel - RefreshSharingProps");

            //    returnList = cachedReturnList.ShallowCopy();
            //    stopWatch.Read(_log, "GetListModel - ShallowCopy");
            //}
            //else
            //{
            var list = _historyListrepo.ReadListForCustomer(catalogInfo, false); // Not returned catalog ID here
                stopWatch.Read(_log, "HistoryListLogicImpl/GetListModel - _listRepo.Read");

            if (list == null)
                return null;
            if (list != null && list.Count==0)
                return null;
            if (list != null && list.Count > 0 && list[0].ListId != Id)
                return null;

            ListModel tempList = list[0];
                stopWatch.Read(_log, "HistoryListLogicImpl/GetListModel - ToListModel");

                returnList = tempList.ShallowCopy();
            //}
            return returnList;
        }
    }
}
