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

        private const string CACHE_LIST_GROUPNAME = "List";
        private const string CACHE_LIST_NAME = "List";
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
        private string GetCacheKeyTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}",
                                 CACHEKEY_PREFIX_TYPELISTOFLISTS,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId,
                                 type,
                                 headerOnly);
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
            return GetListCacheItem<List<string>>(GetCacheKeyLabels(catalogInfo));
        }

        public void AddCachedLabels(UserSelectedContext catalogInfo, List<string> list)
        {
            AddListCacheItem<List<string>>(GetCacheKeyLabels(catalogInfo), CACHETIME_HOURS_LABELS, list);
        }

        public List<ListModel> GetCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly) {
            return GetListCacheItem<List<ListModel>>(GetCacheKeyTypedLists(catalogInfo, type, headerOnly));
        }

        public void AddCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly, List<ListModel> lists)
        {
            AddListCacheItem<List<ListModel>>(GetCacheKeyTypedLists(catalogInfo, type, headerOnly), CACHETIME_HOURS_TYPELISTOFLISTS, lists);
        }

        public List<ListModel> GetCachedCustomerLists(UserSelectedContext catalogInfo) {
            return GetListCacheItem<List<ListModel>>(GetCacheKeyUserLists(catalogInfo));
        }

        public void AddCachedCustomerLists(UserSelectedContext catalogInfo, List<ListModel> lists)
        {
            AddListCacheItem<List<ListModel>>(GetCacheKeyUserLists(catalogInfo), CACHETIME_HOURS_LISTOFLISTS, lists);
        }

        public ListModel GetCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id) {
            return _cache.GetItem<ListModel>(CACHE_LIST_GROUPNAME,
                                             CACHE_LIST_PREFIX,
                                             CACHE_LIST_NAME,
                                             GetCacheKeySpecificLists(catalogInfo, type, Id));
        }

        public void AddCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id, ListModel list)
        {
            AddListCacheItem<ListModel>(GetCacheKeySpecificLists(catalogInfo, type, Id), CACHETIME_HOURS_LIST, list);
        }

        public void RemoveTypeOfListsCache(UserSelectedContext catalogInfo, ListType type)
        {
            RemoveListCacheItem(string.Format("{0}_{1}_{2}_{3}_{4}",
                                              CACHEKEY_PREFIX_TYPELISTOFLISTS,
                                              catalogInfo.BranchId,
                                              catalogInfo.CustomerId,
                                              type,
                                              true));
            RemoveListCacheItem(string.Format("{0}_{1}_{2}_{3}_{4}",
                                              CACHEKEY_PREFIX_TYPELISTOFLISTS,
                                              catalogInfo.BranchId,
                                              catalogInfo.CustomerId,
                                              type,
                                              false));
        }

        public void RemoveSpecificCachedList(ListModel list) {
            RemoveListCacheItem(string.Format("{0}_{1}_{2}_{3}_{4}",
                                              CACHEKEY_PREFIX_LIST,
                                              list.BranchId,
                                              list.CustomerNumber,
                                              list.Type,
                                              list.ListId));
        }

        public void ClearCustomersListCaches(UserProfile user, UserSelectedContext catalogInfo, List<ListModel> lists)
        {
            foreach (var list in lists) {
                // typed lists
                RemoveTypeOfListsCache(catalogInfo, list.Type);

                // specific list
                RemoveListCacheItem(string.Format("{0}_{1}_{2}_{3}_{4}",
                                                  CACHEKEY_PREFIX_LIST,
                                                  list.BranchId,
                                                  list.CustomerNumber,
                                                  list.Type,
                                                  list.ListId));
            }
            // customer lists
            RemoveListCacheItem(string.Format("{0}_{1}_{2}",
                                              CACHEKEY_PREFIX_LISTOFLISTS,
                                              catalogInfo.BranchId,
                                              catalogInfo.CustomerId));

            // always try to remove inventory valuation lists; they are not in the regular rollup
            RemoveListCacheItem(string.Format("{0}_{1}_{2}_{3}",
                                              CACHEKEY_PREFIX_TYPELISTOFLISTS,
                                              catalogInfo.BranchId,
                                              catalogInfo.CustomerId,
                                              ListType.InventoryValuation));

            ClearCustomersLabelsCache(catalogInfo);
        }

        public void ClearCustomersLabelsCache(UserSelectedContext catalogInfo)
        {
            // customer labels
            RemoveListCacheItem(string.Format("{0}_{1}_{2}",
                                              CACHEKEY_PREFIX_LABELS,
                                              catalogInfo.BranchId,
                                              catalogInfo.CustomerId));
        }

        private T GetListCacheItem<T>(string key)
        {
            return _cache.GetItem<T>(CACHE_LIST_GROUPNAME,
                                     CACHE_LIST_PREFIX,
                                     CACHE_LIST_NAME,
                                     key);
        }

        private void AddListCacheItem<T>(string key, int hours, T item)
        {
            _cache.AddItem<T>(CACHE_LIST_GROUPNAME,
                              CACHE_LIST_PREFIX,
                              CACHE_LIST_NAME,
                              key,
                              TimeSpan.FromHours(CACHETIME_HOURS_LIST),
                              item);
        }

        private void RemoveListCacheItem(string key)
        {
            // customer labels
            _cache.RemoveItem(CACHE_LIST_GROUPNAME,
                              CACHE_LIST_PREFIX,
                              CACHE_LIST_NAME,
                              key);
        }

        #endregion
    }
}
