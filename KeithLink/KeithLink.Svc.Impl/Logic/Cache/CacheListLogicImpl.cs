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

        private const string CACHEKEY_PREFIX_TYPELISTOFLISTS = "Lists";

        private const string CACHEKEY_PREFIX_LISTOFLISTS = "Lists";

        private const string CACHEKEY_PREFIX_LIST = "List";

        private const string CACHEKEY_PREFIX_LABELS = "Labels";

        private string CustomersLabelsCacheKey(string customerNumber, string branchId)
        {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_LABELS,
                                 branchId,
                                 customerNumber);
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
            return GetListCacheItem<List<string>>(CustomersLabelsCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId));
        }

        public void AddCachedLabels(UserSelectedContext catalogInfo, List<string> list) {
            AddListCacheItem<List<string>>(CustomersLabelsCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId), HOURS_FOR_LABELS_TO_CACHE, list);
            AddCustomersCachedObjects(catalogInfo.CustomerId, catalogInfo.BranchId, 
                CustomersLabelsCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId));
        }

        public List<ListModel> GetCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly) {
            return GetListCacheItem<List<ListModel>>(TypedListCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId, type, headerOnly));
        }

        public void AddCachedTypedLists(UserSelectedContext catalogInfo, ListType type, bool headerOnly, List<ListModel> lists) {
            AddListCacheItem<List<ListModel>>(TypedListCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId, type, headerOnly), HOURS_FOR_TYPEDLISTS_TO_CACHE, lists);
            AddCustomersCachedObjects(catalogInfo.CustomerId, catalogInfo.BranchId, 
                TypedListCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId, type, headerOnly));
        }

        public List<ListModel> GetCachedCustomerLists(UserSelectedContext catalogInfo) {
            return GetListCacheItem<List<ListModel>>(UserListsCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId));
        }

        public void AddCachedCustomerLists(UserSelectedContext catalogInfo, List<ListModel> lists) {
            AddListCacheItem<List<ListModel>>(UserListsCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId), HOURS_FOR_LISTPAGE_HEADERS_TO_CACHE, lists);
            AddCustomersCachedObjects(catalogInfo.CustomerId, catalogInfo.BranchId, 
                UserListsCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId));
        }

        public ListModel GetCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id) {
            return _cache.GetItem<ListModel>(CACHE_LIST_GROUPNAME,
                                             CACHE_LIST_PREFIX,
                                             CACHE_LIST_NAME,
                                             SpecificListCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId, type, Id));
        }

        public void AddCachedSpecificList(UserSelectedContext catalogInfo, ListType type, long Id, ListModel list) {
            AddListCacheItem<ListModel>(SpecificListCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId, type, Id), HOURS_FOR_A_LIST_TO_CACHE, list);
            AddCustomersCachedObjects(catalogInfo.CustomerId, catalogInfo.BranchId, 
                SpecificListCacheKey(catalogInfo.CustomerId, catalogInfo.BranchId, type, Id));
        }

        public void RemoveTypeOfListsCache(UserSelectedContext catalogInfo, ListType type) {
            RemoveTypeOfListsCache(catalogInfo.CustomerId, catalogInfo.BranchId, type);
        }

        public void RemoveTypeOfListsCache(string customerNumber, string branchId, ListType type)
        {
            RemoveListCacheItem(TypedListCacheKey(customerNumber, branchId, type, true));
            RemoveListCacheItem(TypedListCacheKey(customerNumber, branchId, type, false));
        }

        public void RemoveSpecificCachedList(ListModel list) {
            RemoveListCacheItem(SpecificListCacheKey(list.CustomerNumber,
                                                     list.BranchId,
                                                     list.Type,
                                                     list.ListId));
        }

        public void ClearCustomersListCaches(UserProfile user, UserSelectedContext catalogInfo, List<ListModel> lists)
        {
            ClearCustomersListCaches(user, catalogInfo.CustomerId, catalogInfo.BranchId, lists);
        }

        public void ClearCustomersListCaches(UserProfile user, string customerNumber, string branchId, List<ListModel> lists) {
            foreach (var list in lists) {
                // typed lists
                RemoveTypeOfListsCache(customerNumber, branchId, list.Type);

                // specific list
                RemoveListCacheItem(SpecificListCacheKey(customerNumber, branchId, list.Type, list.ListId));
            }
            // customer lists
            RemoveListCacheItem(UserListsCacheKey(customerNumber, branchId));

            // always try to remove inventory valuation lists; they are not in the regular rollup
            RemoveTypeOfListsCache(customerNumber, branchId, ListType.InventoryValuation);

            ClearCustomersLabelsCache(customerNumber, branchId);

            // try to remove anything registered
            List<string> mylist = GetCustomersCachedObjects(customerNumber, branchId);
            if (mylist != null &&
                mylist.Count > 0) {
                foreach (var str in mylist) {
                    RemoveListCacheItem(str);
                }
            }
            EmptyCustomersCachedObjects(customerNumber, branchId);
        }

        public void ClearCustomersLabelsCache(UserSelectedContext catalog)
        {
            ClearCustomersLabelsCache(catalog.CustomerId, catalog.BranchId);
        }

        public void ClearCustomersLabelsCache(string customerNumber, string branchId) {
            // customer labels
            RemoveListCacheItem(CustomersLabelsCacheKey(customerNumber, branchId));
        }

        private string ContractDictionaryCacheKey(UserSelectedContext catalogInfo)
        {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_CONTRACTDICT,
                                 catalogInfo.BranchId,
                                 catalogInfo.CustomerId);
        }

        private string TypedListCacheKey(string customerNumber, string branchId, ListType type, bool headerOnly)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}",
                                 CACHEKEY_PREFIX_TYPELISTOFLISTS,
                                 branchId,
                                 customerNumber,
                                 type,
                                 headerOnly);
        }

        private string UserListsCacheKey(string customerNumber, string branchId)
        {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_LISTOFLISTS,
                                 branchId,
                                 customerNumber);
        }

        private string SpecificListCacheKey(string customerNumber, string branchId, ListType type, long Id)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}",
                                 CACHEKEY_PREFIX_LIST,
                                 branchId,
                                 customerNumber,
                                 type,
                                 Id);
        }

        private string CustomersCacheObjectsIndexCacheKey(string customerNumber, string branchId)
        {
            return string.Format("{0}_{1}_{2}",
                                 CACHEKEY_PREFIX_LIST,
                                 branchId,
                                 customerNumber);
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

        private List<string> GetCustomersCachedObjects(string customerNumber, string branchId) {
            return GetListCacheItem<List<string>>(CustomersCacheObjectsIndexCacheKey(customerNumber, branchId));
        }

        private void AddCustomersCachedObjects(string customerNumber, string branchId, string key) {
            var list = GetListCacheItem<List<string>>(CustomersCacheObjectsIndexCacheKey(customerNumber, branchId));
            if (list == null) {
                list = new List<string>();
            }
            list.Add(key);
            AddListCacheItem(CustomersCacheObjectsIndexCacheKey(customerNumber, branchId),
                             HOURS_FOR_A_LIST_TO_CACHE,
                             list);
        }

        private void EmptyCustomersCachedObjects(string customerNumber, string branchId) {
            AddListCacheItem(CustomersCacheObjectsIndexCacheKey(customerNumber, branchId),
                             HOURS_FOR_A_LIST_TO_CACHE,
                             new List<string>());
        }
        #endregion
    }
}
