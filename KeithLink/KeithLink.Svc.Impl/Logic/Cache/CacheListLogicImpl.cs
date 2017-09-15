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
    public class CacheListLogicImpl : ICacheListLogic {
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

        private string ContractDictionaryCacheKey(UserSelectedContext catalogInfo) {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_CONTRACTDICT,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId);
        }

        private const string CACHEKEY_PREFIX_TYPELISTOFLISTS = "Lists";

        private string TypedListCacheKey(UserSelectedContext catalogInfo, ListType type, bool headerOnly) {
            return string.Format("{0}_{1}_{2}_{3}_{4}",
                                 CACHEKEY_PREFIX_TYPELISTOFLISTS,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId,
                                 type,
                                 headerOnly);
        }

        private const string CACHEKEY_PREFIX_LISTOFLISTS = "Lists";

        private string UserListsCacheKey(UserSelectedContext catalogInfo) {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_LISTOFLISTS,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId);
        }

        private const string CACHEKEY_PREFIX_LIST = "List";

        private string SpecificListCacheKey(UserSelectedContext catalogInfo, ListType type, long Id) {
            return string.Format("{0}_{1}_{2}_{3}_{4}",
                                 CACHEKEY_PREFIX_LIST,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId,
                                 type,
                                 Id);
        }

        private string CustomersCacheObjectsIndexCacheKey(UserSelectedContext catalogInfo) {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_LIST,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId);
        }

        private const string CACHEKEY_PREFIX_LABELS = "Labels";

        private string CustomersLabelsCacheKey(UserSelectedContext catalogInfo) {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_LABELS,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId);
        }

        private const int HOURS_FOR_CONTRACTDICT_TO_CACHE = 2;
        private const int HOURS_FOR_TYPEDLISTS_TO_CACHE = 2;
        private const int HOURS_FOR_LISTPAGE_HEADERS_TO_CACHE = 2;
        private const int HOURS_FOR_A_LIST_TO_CACHE = 2;
        private const int HOURS_FOR_LABELS_TO_CACHE = 2;
        #endregion

        #region ctor
        public CacheListLogicImpl(ICacheRepository cache) {
            _cache = cache;
        }
        #endregion

        #region methods
        public Dictionary<string, string> GetCachedContractInformation(UserSelectedContext catalogInfo) {
            return _cache.GetItem<Dictionary<string, string>>(CACHE_CONTRACT_GROUPNAME,
                                                              CACHE_CONTRACT_PREFIX,
                                                              CACHE_CONTRACT_NAME,
                                                              ContractDictionaryCacheKey(catalogInfo));
        }

        public void AddCachedContractInformation(UserSelectedContext catalogInfo, Dictionary<string, string> contractdictionary) {
            _cache.AddItem<Dictionary<string, string>>(CACHE_CONTRACT_GROUPNAME,
                                                       CACHE_CONTRACT_PREFIX,
                                                       CACHE_CONTRACT_NAME,
                                                       ContractDictionaryCacheKey(catalogInfo),
                                                       TimeSpan.FromHours(HOURS_FOR_CONTRACTDICT_TO_CACHE),
                                                       contractdictionary);
        }

        public List<string> GetCachedLabels(UserSelectedContext catalogInfo) {
            return GetListCacheItem<List<string>>(CustomersLabelsCacheKey(catalogInfo));
        }

        public void AddCachedLabels(UserSelectedContext catalogInfo, List<string> list) {
            AddListCacheItem<List<string>>(CustomersLabelsCacheKey(catalogInfo), HOURS_FOR_LABELS_TO_CACHE, list);
            AddCustomersCachedObjects(catalogInfo, CustomersLabelsCacheKey(catalogInfo));
        }

        public List<ListModel> GetCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly) {
            return GetListCacheItem<List<ListModel>>(TypedListCacheKey(catalogInfo, type, headerOnly));
        }

        public void AddCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly, List<ListModel> lists) {
            AddListCacheItem<List<ListModel>>(TypedListCacheKey(catalogInfo, type, headerOnly), HOURS_FOR_TYPEDLISTS_TO_CACHE, lists);
            AddCustomersCachedObjects(catalogInfo, TypedListCacheKey(catalogInfo, type, headerOnly));
        }

        public List<ListModel> GetCachedCustomerLists(UserSelectedContext catalogInfo) {
            return GetListCacheItem<List<ListModel>>(UserListsCacheKey(catalogInfo));
        }

        public void AddCachedCustomerLists(UserSelectedContext catalogInfo, List<ListModel> lists) {
            AddListCacheItem<List<ListModel>>(UserListsCacheKey(catalogInfo), HOURS_FOR_LISTPAGE_HEADERS_TO_CACHE, lists);
            AddCustomersCachedObjects(catalogInfo, UserListsCacheKey(catalogInfo));
        }

        public ListModel GetCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id) {
            return _cache.GetItem<ListModel>(CACHE_LIST_GROUPNAME,
                                             CACHE_LIST_PREFIX,
                                             CACHE_LIST_NAME,
                                             SpecificListCacheKey(catalogInfo, type, Id));
        }

        public void AddCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id, ListModel list) {
            AddListCacheItem<ListModel>(SpecificListCacheKey(catalogInfo, type, Id), HOURS_FOR_A_LIST_TO_CACHE, list);
            AddCustomersCachedObjects(catalogInfo, SpecificListCacheKey(catalogInfo, type, Id));
        }

        public void RemoveTypeOfListsCache(UserSelectedContext catalogInfo, ListType type) {
            RemoveListCacheItem(TypedListCacheKey(catalogInfo, type, true));
            RemoveListCacheItem(TypedListCacheKey(catalogInfo, type, false));
        }

        public void RemoveSpecificCachedList(ListModel list) {
            RemoveListCacheItem(SpecificListCacheKey(new UserSelectedContext() {
                                                                                       BranchId = list.BranchId,
                                                                                       CustomerId = list.CustomerNumber
                                                                                   },
                                                         list.Type,
                                                         list.ListId));
        }

        public void ClearCustomersListCaches(UserProfile user, UserSelectedContext catalogInfo, List<ListModel> lists) {
            foreach (var list in lists) {
                // typed lists
                RemoveTypeOfListsCache(catalogInfo, list.Type);

                // specific list
                RemoveListCacheItem(SpecificListCacheKey(catalogInfo, list.Type, list.ListId));
            }
            // customer lists
            RemoveListCacheItem(UserListsCacheKey(catalogInfo));

            // always try to remove inventory valuation lists; they are not in the regular rollup
            RemoveTypeOfListsCache(catalogInfo, ListType.InventoryValuation);

            ClearCustomersLabelsCache(catalogInfo);

            // try to remove anything registered
            List<string> mylist = GetCustomersCachedObjects(catalogInfo);
            if (mylist != null &&
                mylist.Count > 0) {
                foreach (var str in mylist) {
                    RemoveListCacheItem(str);
                }
            }
            EmptyCustomersCachedObjects(catalogInfo);
        }

        public void ClearCustomersLabelsCache(UserSelectedContext catalogInfo) {
            // customer labels
            RemoveListCacheItem(CustomersLabelsCacheKey(catalogInfo));
        }

        private T GetListCacheItem<T>(string key) {
            return _cache.GetItem<T>(CACHE_LIST_GROUPNAME,
                                     CACHE_LIST_PREFIX,
                                     CACHE_LIST_NAME,
                                     key);
        }

        private void AddListCacheItem<T>(string key, int hours, T item) {
            _cache.AddItem<T>(CACHE_LIST_GROUPNAME,
                              CACHE_LIST_PREFIX,
                              CACHE_LIST_NAME,
                              key,
                              TimeSpan.FromHours(HOURS_FOR_A_LIST_TO_CACHE),
                              item);
        }

        private void RemoveListCacheItem(string key) {
            // customer labels
            _cache.RemoveItem(CACHE_LIST_GROUPNAME,
                              CACHE_LIST_PREFIX,
                              CACHE_LIST_NAME,
                              key);
        }

        private List<string> GetCustomersCachedObjects(UserSelectedContext catalogInfo) {
            return GetListCacheItem<List<string>>(CustomersCacheObjectsIndexCacheKey(catalogInfo));
        }

        private void AddCustomersCachedObjects(UserSelectedContext catalogInfo, string key) {
            var list = GetListCacheItem<List<string>>(CustomersCacheObjectsIndexCacheKey(catalogInfo));
            if (list == null) {
                list = new List<string>();
            }
            list.Add(key);
            AddListCacheItem(CustomersCacheObjectsIndexCacheKey(catalogInfo),
                             HOURS_FOR_A_LIST_TO_CACHE,
                             list);
        }

        private void EmptyCustomersCachedObjects(UserSelectedContext catalogInfo) {
            AddListCacheItem(CustomersCacheObjectsIndexCacheKey(catalogInfo),
                             HOURS_FOR_A_LIST_TO_CACHE,
                             new List<string>());
        }
        #endregion
    }
}
