using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic.Cache
{
    public class CacheListLogicImpl : ICacheListLogic
    {
        #region attributes
        private readonly ICacheRepository _cache;

        private Dictionary<string, string> _contractdictionary;

        private const string CACHE_CONTRACT_GROUPNAME = "ContractInformation";
        private const string CACHE_CONTRACT_NAME = "ContractInformation";
        private const string CACHE_CONTRACT_PREFIX = "Default";

        private const string CACHE_LIST_GROUPNAME = "ContractInformation";
        private const string CACHE_LIST_NAME = "ContractInformation";
        private const string CACHE_LIST_PREFIX = "Default";

        private const string CACHEKEY_PREFIX_CONTRACTDICT = "ContractDictionary";
        private string GetCacheKeyContractDictionary(UserSelectedContext catalogInfo)
        {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_CONTRACTDICT,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId);
        }

        private const string CACHEKEY_PREFIX_TYPELISTOFLISTS = "Lists";
        private string GetCacheKeyTypedLists(UserSelectedContext catalogInfo, ListType type)
        {
            return string.Format("{0}_{1}_{2}_{3}",
                                 CACHEKEY_PREFIX_TYPELISTOFLISTS,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId,
                                 type);
        }

        private const string CACHEKEY_PREFIX_LISTOFLISTS = "Lists";
        private string GetCacheKeyUserLists(UserSelectedContext catalogInfo)
        {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_LISTOFLISTS,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId);
        }

        private const string CACHEKEY_PREFIX_LIST = "List";
        private string GetCacheKeySpecificLists(UserSelectedContext catalogInfo, ListType type, long Id)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}",
                                                                           CACHEKEY_PREFIX_LIST,
                                                                           catalogInfo.BranchId,
                                                                           catalogInfo.CustomerId,
                                                                           type,
                                                                           Id);
        }

        private const string CACHEKEY_PREFIX_LABELS = "Labels";
        private string GetCacheKeyLabels(UserSelectedContext catalogInfo)
        {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_LABELS,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId);
        }

        private const int CACHETIME_HOURS_CONTRACTDICT = 2;
        private const int CACHETIME_HOURS_TYPELISTOFLISTS = 2;
        private const int CACHETIME_HOURS_LISTOFLISTS = 2;
        private const int CACHETIME_HOURS_LIST = 2;
        private const int CACHETIME_HOURS_LABELS = 2;
        #endregion

        #region ctor
        public CacheListLogicImpl(ICacheRepository cache)
        {
            _cache = cache;
        }
        #endregion

        #region methods
        public Dictionary<string, string> GetCachedContractInformation(UserSelectedContext catalogInfo) {
            return _cache.GetItem<Dictionary<string, string>>(CACHE_CONTRACT_GROUPNAME,
                                                              CACHE_CONTRACT_PREFIX,
                                                              CACHE_CONTRACT_NAME,
                                                              GetCacheKeyContractDictionary(catalogInfo));
        }

        public void AddCachedContractInformation(UserSelectedContext catalogInfo, Dictionary<string, string> contractdictionary)
        {
            _cache.AddItem<Dictionary<string, string>>(CACHE_CONTRACT_GROUPNAME,
                                                       CACHE_CONTRACT_PREFIX,
                                                       CACHE_CONTRACT_NAME,
                                                       GetCacheKeyContractDictionary(catalogInfo),
                                                       TimeSpan.FromHours(CACHETIME_HOURS_CONTRACTDICT),
                                                       contractdictionary);
        }

        public List<string> GetCachedLabels(UserSelectedContext catalogInfo)
        {
            return _cache.GetItem<List<string>>(CACHE_LIST_GROUPNAME,
                                                CACHE_LIST_PREFIX,
                                                CACHE_LIST_NAME,
                                                GetCacheKeyLabels(catalogInfo));
        }

        public void AddCachedLabels(UserSelectedContext catalogInfo, List<string> list)
        {
            _cache.AddItem<List<string>>(CACHE_LIST_GROUPNAME,
                                         CACHE_LIST_PREFIX,
                                         CACHE_LIST_NAME,
                                         GetCacheKeyLabels(catalogInfo),
                                         TimeSpan.FromHours(CACHETIME_HOURS_LABELS),
                                         list);
        }

        public List<ListModel> GetCachedTypedLists(UserSelectedContext catalogInfo, ListType type) {
            return _cache.GetItem<List<ListModel>>(CACHE_LIST_GROUPNAME,
                                                   CACHE_LIST_PREFIX,
                                                   CACHE_LIST_NAME,
                                                   GetCacheKeyTypedLists(catalogInfo, type));
        }

        public void AddCachedTypedLists(UserSelectedContext catalogInfo, ListType type, List<ListModel> lists)
        {
            _cache.AddItem<List<ListModel>>(CACHE_LIST_GROUPNAME,
                                            CACHE_LIST_PREFIX,
                                            CACHE_LIST_NAME,
                                            GetCacheKeyTypedLists(catalogInfo, type),
                                            TimeSpan.FromHours(CACHETIME_HOURS_TYPELISTOFLISTS),
                                            lists);
        }

        public List<ListModel> GetCachedCustomerLists(UserSelectedContext catalogInfo) {
            return _cache.GetItem<List<ListModel>>(CACHE_LIST_GROUPNAME,
                                                   CACHE_LIST_PREFIX,
                                                   CACHE_LIST_NAME,
                                                   GetCacheKeyUserLists(catalogInfo));
        }

        public void AddCachedCustomerLists(UserSelectedContext catalogInfo, List<ListModel> lists)
        {
            _cache.AddItem<List<ListModel>>(CACHE_LIST_GROUPNAME,
                                                    CACHE_LIST_PREFIX,
                                                    CACHE_LIST_NAME,
                                                    GetCacheKeyUserLists(catalogInfo),
                                                    TimeSpan.FromHours(CACHETIME_HOURS_LISTOFLISTS),
                                                    lists);
        }

        public ListModel GetCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id) {
            return _cache.GetItem<ListModel>(CACHE_LIST_GROUPNAME,
                                             CACHE_LIST_PREFIX,
                                             CACHE_LIST_NAME,
                                             GetCacheKeySpecificLists(catalogInfo, type, Id));
        }

        public void AddCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id, ListModel list)
        {
            _cache.AddItem<ListModel>(CACHE_LIST_GROUPNAME,
                                      CACHE_LIST_PREFIX,
                                      CACHE_LIST_NAME,
                                      GetCacheKeySpecificLists(catalogInfo, type, Id),
                                      TimeSpan.FromHours(CACHETIME_HOURS_LIST),
                                      list);
        }

        public void ClearCustomersListCaches(UserProfile user, UserSelectedContext catalogInfo, List<ListModel> lists)
        {
            //_cache.ResetAllItems(CACHE_LIST_GROUPNAME,
            //                     CACHE_LIST_PREFIX,
            //                     CACHE_LIST_NAME);

            foreach (var list in lists) {
                // typed lists
                _cache.RemoveItem(CACHE_LIST_GROUPNAME,
                                  CACHE_LIST_PREFIX,
                                  CACHE_LIST_NAME,
                                  string.Format("{0}_{1}_{2}_{3}",
                                                CACHEKEY_PREFIX_TYPELISTOFLISTS,
                                                list.BranchId,
                                                list.CustomerNumber,
                                                list.Type));

                // specific list
                _cache.RemoveItem(CACHE_LIST_GROUPNAME,
                                  CACHE_LIST_PREFIX,
                                  CACHE_LIST_NAME,
                                  string.Format("{0}_{1}_{2}_{3}_{4}",
                                                CACHEKEY_PREFIX_LIST,
                                                list.BranchId,
                                                list.CustomerNumber,
                                                list.Type,
                                                list.ListId));
            }
            // customer lists
            _cache.RemoveItem(CACHE_LIST_GROUPNAME,
                              CACHE_LIST_PREFIX,
                              CACHE_LIST_NAME,
                              string.Format("{0}_{1}_{2}",
                                            CACHEKEY_PREFIX_LISTOFLISTS,
                                            lists[0].BranchId,
                                            lists[0].CustomerNumber));

            // customer labels
            _cache.RemoveItem(CACHE_LIST_GROUPNAME,
                              CACHE_LIST_PREFIX,
                              CACHE_LIST_NAME,
                              string.Format("{0}_{1}_{2}",
                                            CACHEKEY_PREFIX_LABELS,
                                            lists[0].BranchId,
                                            lists[0].CustomerNumber));
        }
        #endregion
    }
}
