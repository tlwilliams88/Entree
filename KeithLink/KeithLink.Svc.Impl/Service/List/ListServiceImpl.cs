﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Models.Reports;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Models.EF;
using Microsoft.Practices.ObjectBuilder2;

namespace KeithLink.Svc.Impl.Service.List
{
    public class ListServiceImpl : IListService
    {
        #region attributes
        private readonly ICacheRepository _cache;
        private readonly IContractListLogic _contractListLogic;
        private readonly IHistoryListLogic _historyListLogic;
        private readonly IFavoritesListLogic _favoritesLogic;
        private readonly IRecentlyViewedListLogic _recentlyViewedLogic;
        private readonly IRecentlyOrderedListLogic _recentlyOrderedLogic;
        private readonly IRecommendedItemsListLogic _recommendedItemsLogic;
        private readonly IMandatoryItemsListLogic _mandatoryItemsLogic;
        private readonly IInventoryValuationListLogic _inventoryValuationLogic;
        private readonly IRemindersListLogic _reminderItemsLogic;
        private readonly ICustomListLogic _customListLogic;
        private readonly INotesListLogic _notesLogic;
        private readonly ICatalogLogic _catalogLogic;
        private readonly IExternalCatalogRepository _externalCatalogRepo;
        private readonly IItemHistoryRepository _itemHistoryRepo;
        private readonly IPriceLogic _priceLogic;
        private readonly IProductImageRepository _productImageRepo;
        private readonly IItemBarcodeImageRepository _barcodeImageRepo;
        private readonly ICustomInventoryItemsRepository _customInventoryRepo;
        private readonly IEventLogRepository _log;

        private Dictionary<string, string> _contractdictionary;

        private const string CACHE_CONTRACT_GROUPNAME = "ContractInformation";
        private const string CACHE_CONTRACT_NAME = "ContractInformation";
        private const string CACHE_CONTRACT_PREFIX = "Default";
        #endregion

        #region ctor
        public ListServiceImpl( IHistoryListLogic historyListLogic, ICatalogLogic catalogLogic, INotesListLogic notesLogic,
                                IItemHistoryRepository itemHistoryRepo, IFavoritesListLogic favoritesLogic, IPriceLogic priceLogic,
                                IRecentlyViewedListLogic recentlyViewedLogic, IRecentlyOrderedListLogic recentlyOrderedLogic, 
                                IRecommendedItemsListLogic recommendedItemsLogic, IRemindersListLogic reminderItemsLogic,
                                IProductImageRepository productImageRepo, IExternalCatalogRepository externalCatalogRepo, IItemBarcodeImageRepository barcodeImageRepo,
                                IMandatoryItemsListLogic mandatoryItemsLogic, IInventoryValuationListLogic inventoryValuationLogic,
                                IContractListLogic contractListLogic, ICustomListLogic customListLogic, ICacheRepository cache,
                                IEventLogRepository log, ICustomInventoryItemsRepository customInventoryRepo)
        {
            _cache = cache;
            // specific lists -
            _contractListLogic = contractListLogic;
            _historyListLogic = historyListLogic;
            _favoritesLogic = favoritesLogic;
            _recentlyViewedLogic = recentlyViewedLogic;
            _recentlyOrderedLogic = recentlyOrderedLogic;
            _recommendedItemsLogic = recommendedItemsLogic;
            _reminderItemsLogic = reminderItemsLogic;
            _mandatoryItemsLogic = mandatoryItemsLogic;
            _inventoryValuationLogic = inventoryValuationLogic;
            _customListLogic = customListLogic;
            _notesLogic = notesLogic;
            _catalogLogic = catalogLogic;
            _externalCatalogRepo = externalCatalogRepo;
            _itemHistoryRepo = itemHistoryRepo;
            _priceLogic = priceLogic;
            _productImageRepo = productImageRepo;
            _customInventoryRepo = customInventoryRepo;
            _barcodeImageRepo = barcodeImageRepo;
            _log = log;
        }
        #endregion

        #region methods
        public Dictionary<string, string> GetContractInformation(UserSelectedContext catalogInfo)
        {
            Dictionary<string, string> contractdictionary = new Dictionary<string, string>();

            Dictionary<string, string> cachedContractdictionary = _cache.GetItem<Dictionary<string, string>>(CACHE_CONTRACT_GROUPNAME,
                                                                                                             CACHE_CONTRACT_PREFIX,
                                                                                                             CACHE_CONTRACT_NAME,
                                                                                                             string.Format("ContractDictionary_{0}_{1}",
                                                                                                                           catalogInfo.BranchId,
                                                                                                                           catalogInfo.CustomerId));

            if (cachedContractdictionary == null)
            {
                ListModel contract = _contractListLogic.GetListModel(null, catalogInfo, 0);
                if (contract != null)
                {
                    // When we apply contract categories to other lists, on contracts that have the same itemnumber 
                    // for case and package lines have the same itemnumber twice.So the dictionary blows up trying 
                    // to put the two entries for the same itemnumber in...
                    // The dictionary just applies the category to that same item used in other lists. So the only 
                    // negative is if they specify the itemnumber/case as being in a different category than the 
                    // item /package combination. Nothing changes in how it is used in an order or anything.
                    contractdictionary = contract.Items
                                                 .GroupBy(li => li.ItemNumber, StringComparer.CurrentCultureIgnoreCase)
                                                 .ToDictionary(g => g.Key,
                                                               g => g.First().Category.Trim());
                }
                _cache.AddItem<Dictionary<string, string>>(CACHE_CONTRACT_GROUPNAME,
                                                           CACHE_CONTRACT_PREFIX,
                                                           CACHE_CONTRACT_NAME,
                                                           string.Format("ContractDictionary_{0}_{1}",
                                                                         catalogInfo.BranchId,
                                                                         catalogInfo.CustomerId), TimeSpan.FromHours(2), contractdictionary);

            }
            else
            {
                contractdictionary = cachedContractdictionary;
            }

            return contractdictionary;
        }

        public ItemHistory[] GetItemsHistoryList(UserSelectedContext userContext, string[] itemNumbers)
        {
            ItemHistory[] itemStatistics = _itemHistoryRepo.Read(f => f.BranchId.Equals(userContext.BranchId) &&
                                                                      f.CustomerNumber.Equals(userContext.CustomerId))
                                                           .Where(f => itemNumbers.Contains(f.ItemNumber))
                                                           .ToArray();
            return itemStatistics;
        }

        public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false)
        {
            List<ListModel> returnList = new List<ListModel>();

            switch (type)
            {
                case ListType.Custom:
                    returnList.TryAddRange(_customListLogic.ReadLists(user, catalogInfo, headerOnly));
                    break;

                case ListType.Favorite:
                    returnList.TryAdd(_favoritesLogic.GetFavoritesList(user.UserId, catalogInfo, headerOnly));
                    break;

                case ListType.Contract:
                    returnList.TryAdd(_contractListLogic.GetListModel(user, catalogInfo, 0));
                    break;

                case ListType.Notes: 
                    returnList.TryAdd(_notesLogic.GetList(catalogInfo));
                    break;

                case ListType.Worksheet:
                    returnList.TryAdd(_historyListLogic.GetListModel(user, catalogInfo, 0));
                    break;

                // no contract items added lists
                // no contract items deleted lists

                case ListType.Reminder:
                    returnList.TryAdd(_reminderItemsLogic.GetListModel(user, catalogInfo, 0));
                    break;

                case ListType.Mandatory:
                    returnList.TryAdd(_mandatoryItemsLogic.ReadList(user, catalogInfo, headerOnly));
                    break;

                case ListType.RecommendedItems:
                    returnList.TryAdd(_recommendedItemsLogic.ReadList(user, catalogInfo, headerOnly));
                    break;

                case ListType.InventoryValuation:
                    returnList.TryAddRange(_inventoryValuationLogic.ReadLists(user, catalogInfo, headerOnly));
                    break;

                case ListType.RecentlyOrdered:
                    returnList.TryAdd(_recentlyOrderedLogic.ReadList(user, catalogInfo, headerOnly));
                    break;

                case ListType.RecentlyViewed:
                    returnList.TryAdd(_recentlyViewedLogic.ReadList(user, catalogInfo, headerOnly));
                    break;

                    //case ListType.CustomInventory: //uses its own controller and works a little differently
                    //    returnList.Add(_customListLogic.GetListModel(user, catalogInfo, 0));
                    //    break;
            }

            if (returnList.Count > 0) {
                FillOutProducts(user, catalogInfo, returnList, true);
            }

            return returnList;
        }

        private ListModel ReadListById(UserProfile user, UserSelectedContext catalogInfo, long Id, ListType type)
        {
            ListModel tempList = null;
            switch (type)
            {
                case ListType.Custom:
                    tempList = _customListLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.Favorite:
                    tempList = _favoritesLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.Contract:
                    tempList = _contractListLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.RecentlyViewed:
                    tempList = _recentlyViewedLogic.ReadList(user, catalogInfo, false);
                    break;

                case ListType.Notes:
                    tempList = _notesLogic.GetList(catalogInfo);
                    break;

                case ListType.Worksheet:
                    tempList = _historyListLogic.GetListModel(user, catalogInfo, Id);
                    break;

                //// no contract items added lists
                //// no contract items deleted lists

                case ListType.Reminder:
                    tempList = _reminderItemsLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.Mandatory:
                    tempList = _mandatoryItemsLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.RecommendedItems:
                    tempList = _recommendedItemsLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.InventoryValuation:
                    tempList = _inventoryValuationLogic.ReadList(Id, catalogInfo, false);
                    break;

                case ListType.RecentlyOrdered:
                    ////    returnList.Add(_recentlyOrderedLogic.GetListModel(user, catalogInfo, 0));
                    break;

                    ////case ListType.CustomInventory: //uses its own controller and works a little differently
                    ////    returnList.Add(_customListLogic.GetListModel(user, catalogInfo, 0));
                    ////    break;

            }

            if (tempList != null && tempList.Items != null && tempList.Items.Count > 0)
            {
                FillOutProducts(user, catalogInfo, new List<ListModel>() { tempList }, true);
            }

            return tempList;
        }

        public List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
        {
            List<ListModel> list = new List<ListModel>();
            
            if(_contractdictionary == null) { _contractdictionary = GetContractInformation(catalogInfo); }

            AddListsIfNotNull(user, catalogInfo, ListType.Worksheet, list, headerOnly);
            AddListsIfNotNull(user, catalogInfo, ListType.Contract, list, headerOnly);
            AddListsIfNotNull(user, catalogInfo, ListType.Favorite, list, headerOnly);
            AddListsIfNotNull(user, catalogInfo, ListType.Reminder, list, headerOnly);
            AddListsIfNotNull(user, catalogInfo, ListType.RecommendedItems, list, headerOnly);
            AddListsIfNotNull(user, catalogInfo, ListType.Mandatory, list, headerOnly);
            AddListsIfNotNull(user, catalogInfo, ListType.Custom, list, headerOnly);

            return list;
        }

        private void AddListsIfNotNull(UserProfile user, UserSelectedContext catalogInfo, ListType type, List<ListModel> list, bool headerOnly) {
            List<ListModel> tmpList = ReadListByType(user, catalogInfo, type, headerOnly);
            if (tmpList != null && tmpList.Count > 0) {
                list.AddRange(tmpList);
            }
        }

        public List<string> ReadLabels(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<ListModel> list = new List<ListModel>();

            list.AddRange(ReadListByType(user, catalogInfo, ListType.Favorite, false));
            list.AddRange(ReadListByType(user, catalogInfo, ListType.Custom, false));

            List<ListItemModel> items = new List<ListItemModel>();
            foreach (ListModel lst in list) {
                if (lst.Items != null &&
                    lst.Items.Count > 0) {
                    items.AddRange(lst.Items);
                }
            }
            List<string> labels = items.Select(l => l.Label).Distinct().Where(x => x != null).ToList();

            labels.Sort();

            return labels;
        }

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, ListType type, long Id, bool includePrice = true)
        {
            return ReadListById(user, catalogInfo, Id, type);
        }

        public PagedListModel ReadPagedList(UserProfile user,
                                            UserSelectedContext catalogInfo,
                                            ListType type,
                                            long Id,
                                            Core.Models.Paging.PagingModel paging)
        {
            System.Diagnostics.Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(gettiming: false);
            ListModel returnList = null;

            returnList = ReadListById(user, catalogInfo, Id, type);
            stopWatch.Read(_log, "ReadPagedList - GetListModel");

            PagedListModel pagedList = null;
            if (returnList != null && returnList.Items != null)
            {
                pagedList = returnList.ToPagedList(paging);
            }
            else
            {
                pagedList = new PagedListModel();
            }
            stopWatch.Read(_log, "ReadPagedList - ToPagedList");

            return pagedList;
        }

        public List<RecentItem> ReadRecent(UserProfile user, UserSelectedContext catalogInfo)
        {
            var list = ReadListByType(user, catalogInfo, ListType.RecentlyViewed, false);

            if (list != null)
            {
                var returnItems = list.SelectMany(i => i.Items.Select(l => new RecentItem() { ItemNumber = l.ItemNumber, ModifiedOn = l.ModifiedUtc }))
                                      .ToList();

                LookupProductDetails(user, catalogInfo, returnItems);

                returnItems.ForEach(delegate (RecentItem item)
                {
                    item.Images = _productImageRepo.GetImageList(item.ItemNumber).ProductImages;
                });

                return returnItems.OrderByDescending(l => l.ModifiedOn)
                                  .ToList();
            }
            return null;
        }

        public RecentNonBEKList ReadRecentOrder(UserProfile user, UserSelectedContext catalogInfo, string catalog)
        {
            ListModel recentOrders = _recentlyOrderedLogic.ReadList(user,
                new UserSelectedContext() { CustomerId = catalogInfo.CustomerId, BranchId = catalog }, false);

            // Identify specific warehouse - needed for product lookup
            Dictionary<string, string> externalCatalogDict =
                _externalCatalogRepo.ReadAll().ToDictionary(e => e.BekBranchId.ToLower(), e => e.ExternalBranchId);

            List<RecentNonBEKItem> returnItems = new List<RecentNonBEKItem>();
            if (recentOrders != null &&
                externalCatalogDict != null) {
                returnItems = recentOrders.Items
                    .Select(l => new RecentNonBEKItem()
                    {
                        ItemNumber = l.ItemNumber,
                        CatalogId = externalCatalogDict[catalogInfo.BranchId.ToLower()],
                        ModifiedOn = l.ModifiedUtc
                    })
                    .ToList();
            }

            if (returnItems.Count > 0)
            {
                PopulateProductDetails(returnItems);

                returnItems.ForEach(delegate (RecentNonBEKItem item)
                {
                    if (item.Upc != null)
                    {
                        item.Images = _productImageRepo.GetImageList(item.Upc, false).ProductImages;
                    }
                });
            }

            return new RecentNonBEKList() { Catalog = catalog, Items = returnItems };
        }

        public ListModel UpdateList(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                                    ListModel list) {
            ListModel retVal = null;

            switch (type) {
                case ListType.Worksheet:
                case ListType.Contract:
                    // cannot add items to contracts or worksheets
                    break;
                case ListType.Favorite:
                    retVal = _favoritesLogic.SaveList(user, catalogInfo, list);
                    break;
                case ListType.Reminder:
                    retVal = _reminderItemsLogic.SaveList(user, catalogInfo, list);
                    break;
                //case ListType.RecommendedItems:
                //    _recommendedItemsLogic.SaveDetail(catalogInfo, item.ToRecommendedItemsListDetail(headerId));
                //    break;
                case ListType.Mandatory:
                    retVal = _mandatoryItemsLogic.SaveList(user, catalogInfo, list);
                    break;
                case ListType.Custom:
                    retVal = _customListLogic.SaveList(user, catalogInfo, list);
                    break;
                //case ListType.Recent:
                //    break;
                //case ListType.Notes:
                //    break;
                case ListType.InventoryValuation:
                    retVal = _inventoryValuationLogic.SaveList(user, catalogInfo, list);

                    FillOutProducts(user, catalogInfo, retVal, true);

                    break;
                //case ListType.RecentlyOrdered:
                //    break;
            }

            return retVal;
        }

        public void SaveItem(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                             long headerId, ListItemModel item) {
            if (item.Position == 0)
            {
                ListModel list = ReadListById(user, catalogInfo, headerId, type);
                if (list != null) {
                    if (list.Items != null &&
                        list.Items.Count > 0) {
                        item.Position = list.Items.Max(i => i.Position) + 1;
                    } else {
                        item.Position = 1;
                    }
                }
            }

            switch (type) {
                case ListType.Worksheet:
                case ListType.Contract:
                    // cannot add items to contracts or worksheets
                    break;
                case ListType.Favorite:
                    _favoritesLogic.Save(user, catalogInfo, item.ToFavoritesListDetail(headerId));
                    break;
                case ListType.Reminder:
                    _reminderItemsLogic.Save(catalogInfo, item.ToReminderItemsListDetail(headerId));
                    break;
                case ListType.RecommendedItems:
                    _recommendedItemsLogic.SaveDetail(catalogInfo, item.ToRecommendedItemsListDetail(headerId));
                    break;
                case ListType.Mandatory:
                    _mandatoryItemsLogic.SaveDetail(catalogInfo, item.ToMandatoryItemsListDetail(headerId));
                    break;
                case ListType.Custom:
                    _customListLogic.SaveItem(item.ToCustomListDetail(headerId));
                    break;
                case ListType.Recent:
                    break;
                case ListType.Notes:
                    _notesLogic.SaveNote(catalogInfo, item);
                    break;
                case ListType.InventoryValuation:
                    _inventoryValuationLogic.SaveItem(user, catalogInfo, headerId, item.ToInventoryValuationListDetail(headerId));
                    break;
                case ListType.RecentlyOrdered:
                    break;
            }
        }

        public void SaveItems(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                              long headerId, List<ListItemModel> items)
        {
            foreach (var item in items) {
                SaveItem(user, catalogInfo, type, headerId, item);
            }
        }

        public ListModel CreateList(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                                      ListModel list) {
            long id = 0;
            switch (type)
            {
                case ListType.RecommendedItems:
                    _recommendedItemsLogic.CreateList(catalogInfo);
                    break;
                case ListType.Reminder:
                    _reminderItemsLogic.SaveList(user, catalogInfo, list);
                    break;
                case ListType.Mandatory:
                    _mandatoryItemsLogic.CreateList(user, catalogInfo);
                    break;
                case ListType.Custom:
                    id = _customListLogic.CreateOrUpdateList(user, catalogInfo, 0, list.Name, true);
                    break;
                case ListType.InventoryValuation:
                    id = _inventoryValuationLogic.CreateOrUpdateList(user, catalogInfo, 0, list.Name, true);
                    break;
                case ListType.Favorite:
                    id = _favoritesLogic.CreateList(user, catalogInfo);
                    break;
            }

            if (list.Items != null &&
                list.Items.Count > 0) {
                SaveItems(user,catalogInfo, type, id, list.Items);
            }

            return ReadList(user, catalogInfo, type, id, true);
        }

        public List<ListModel> CopyList(UserProfile user, UserSelectedContext catalogInfo, ListCopyShareModel copyListModel) {
            ListModel list = ReadList(user, catalogInfo, copyListModel.Type, copyListModel.ListId);

            List<ListModel> results = new List<ListModel>();

            foreach (var customer in copyListModel.Customers) {
                results.Add(CopyList(user, new UserSelectedContext()
                {
                    BranchId = customer.CustomerBranch,
                    CustomerId = customer.CustomerNumber
                }, list));
            }

            return results;
        }

        private ListModel CopyList(UserProfile user, UserSelectedContext catalogInfo, ListModel list) {

            var copyList = list.NewCopy();
            
            var newList = CreateList(user,
                                     catalogInfo,
                                     ListType.Custom,
                                     copyList);

            return ReadListById(user, catalogInfo, newList.ListId, newList.Type);
        }

        public void DeleteList(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                                      ListModel list)
        {
            switch (type)
            {
                case ListType.Custom:
                    _customListLogic.DeleteList(user, catalogInfo, list);
                    break;
                case ListType.InventoryValuation:
                    long x = _inventoryValuationLogic.CreateOrUpdateList(user, catalogInfo, list.ListId, list.Name, false);
                    break;
            }
        }

        private void PopulateProductDetails(List<RecentNonBEKItem> returnList)
        {
            if (returnList == null)
                return;

            var products = _catalogLogic.GetProductsByIds(returnList[0].CatalogId,
                                                          returnList.Select(i => i.ItemNumber)
                                                                    .Distinct()
                                                                    .ToList());
            returnList.ForEach(delegate (RecentNonBEKItem item) {
                var product = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber))
                                               .FirstOrDefault();
                if (product != null)
                {
                    item.Name = product.Name;
                    item.Upc = product.UPC;
                }
            });
        }

        private void FillOutProducts(UserProfile user, UserSelectedContext catalogInfo, ListModel returnList, bool getprices) {
            List<ListModel> tempList = new List<ListModel>() { returnList };

            FillOutProducts(user, catalogInfo, tempList, getprices);
        }

        private void FillOutProducts(UserProfile user, UserSelectedContext catalogInfo, List<ListModel> returnList, bool getprices)
        {
//Lookup product details for each item
            returnList.ForEach(delegate(ListModel listItem) {
                if (listItem != null) {
                    LookupProductDetails(user, listItem, catalogInfo);
                }
            });

            if (getprices)
            {
                //Lookup prices for each item
                foreach (var tempList in returnList)
                {
                    if (tempList != null &&
                        tempList.Items != null) {
                        LookupPrices(user, tempList.Items, catalogInfo);
                    }
                }
            }
        }

        private void LookupProductDetails(UserProfile user, UserSelectedContext catalogInfo, List<RecentItem> list)
        {
            if (list == null || list.Count == 0)
                return;

            ProductsReturn products = new ProductsReturn() { Products = new List<Product>() };

            {
                var batch = list.Select(i => i.ItemNumber)
                                .ToList();

                var tempProducts = _catalogLogic.GetProductsByIds(catalogInfo.BranchId, batch);

                products.Products.AddRange(tempProducts.Products);
            }

            var productHash = products.Products.GroupBy(p => p.ItemNumber)
                                               .Select(i => i.First())
                                               .ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(list, listItem =>
            {
                var prod = productHash.ContainsKey(listItem.ItemNumber) ? productHash[listItem.ItemNumber] : null;

                if (prod != null)
                {
                    listItem.Name = prod.Name;
                }
            });
        }

        private void LookupProductDetails(UserProfile user, ListModel list, UserSelectedContext catalogInfo)
        {
            if (list.Items == null || list.Items.Count == 0)
                return;

            if (_contractdictionary == null) { _contractdictionary = GetContractInformation(catalogInfo); }


            ProductsReturn products = new ProductsReturn() { Products = new List<Product>() };

            {
                var batch = list.Items.Select(i => i.ItemNumber)
                                      .ToList();

                var tempProducts = _catalogLogic.GetProductsByIds(catalogInfo.BranchId, batch);

                if (tempProducts != null) {
                    products.Products.AddRange(tempProducts.Products);
                }
            }

            var productHash = products.Products.GroupBy(p => p.ItemNumber)
                                               .Select(i => i.First())
                                               .ToDictionary(p => p.ItemNumber);
            Dictionary<long, CustomInventoryItem> customItemHash = GetCustomItemHash(catalogInfo);

            List<ItemHistory> itemStatistics = _itemHistoryRepo.Read(f => f.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                          f.CustomerNumber.Equals(catalogInfo.CustomerId))
                                                               .ToList();

            Parallel.ForEach(list.Items, listItem =>
            {

                if (listItem.CustomInventoryItemId > 0) {
                    if(customItemHash != null && customItemHash.ContainsKey(listItem.CustomInventoryItemId)) {
                        CustomInventoryItem item = customItemHash[listItem.CustomInventoryItemId];
                        listItem.IsValid = true;
                        listItem.Name = item.Name;
                        listItem.BrandExtendedDescription = item.Brand;
                        listItem.Label = item.Label;
                        listItem.Pack = item.Pack;
                        listItem.Size = item.Size;
                        listItem.PackSize = $"{item.Pack} / {item.Size}";
                        listItem.VendorItemNumber = item.Vendor;
                        listItem.Supplier = item.Supplier;
                        listItem.CasePriceNumeric = (double)item.CasePrice;
                        listItem.PackagePriceNumeric = (double)item.PackagePrice;
                        listItem.Each = item.Each;
                    }
                } else {
                    var prod = productHash.ContainsKey(listItem.ItemNumber) ? productHash[listItem.ItemNumber] : null;

                    if (prod != null) {
                        listItem.IsValid = true;
                        listItem.Name = prod.Name;
                        listItem.Pack = prod.Pack;
                        listItem.Size = prod.Size;
                        listItem.PackSize = $"{prod.Pack} / {prod.Size}";
                        listItem.Size = prod.Size;
                        listItem.BrandExtendedDescription = prod.BrandExtendedDescription;
                        listItem.Description = prod.Description;
                        listItem.Brand = prod.BrandExtendedDescription;
                        listItem.ReplacedItem = prod.ReplacedItem;
                        listItem.ReplacementItem = prod.ReplacementItem;
                        listItem.NonStock = prod.NonStock;
                        listItem.ProprietaryCustomers = prod.ProprietaryCustomers;
                        listItem.ChildNutrition = prod.ChildNutrition;
                        listItem.SellSheet = prod.SellSheet;
                        listItem.CatchWeight = prod.CatchWeight;
                        listItem.ItemClass = prod.ItemClass;
                        listItem.CategoryCode = prod.CategoryCode;
                        listItem.SubCategoryCode = prod.SubCategoryCode;
                        listItem.CategoryName = prod.CategoryName;
                        listItem.UPC = prod.UPC;
                        listItem.VendorItemNumber = prod.VendorItemNumber;
                        listItem.Cases = prod.Cases;
                        listItem.Kosher = prod.Kosher;
                        listItem.ManufacturerName = prod.ManufacturerName;
                        listItem.ManufacturerNumber = prod.ManufacturerNumber;
                        listItem.AverageWeight = prod.AverageWeight;
                        listItem.TempZone = prod.TempZone;
                        listItem.IsSpecialtyCatalog = prod.CatalogId.StartsWith("UNFI", StringComparison.InvariantCultureIgnoreCase);
                        listItem.Unfi = prod.Unfi;
                        if (prod.Nutritional != null)
                        {
                            listItem.StorageTemp = prod.Nutritional.StorageTemp;
                            listItem.Nutritional = new Nutritional()
                            {
                                CountryOfOrigin = prod.Nutritional.CountryOfOrigin,
                                GrossWeight = prod.Nutritional.GrossWeight,
                                HandlingInstructions = prod.Nutritional.HandlingInstructions,
                                Height = prod.Nutritional.Height,
                                Length = prod.Nutritional.Length,
                                Ingredients = prod.Nutritional.Ingredients,
                                Width = prod.Nutritional.Width
                            };
                        }
                        listItem.ItemStatistics = new KeithLink.Svc.Core.Models.Customers.ItemHistoryModel()
                        {
                            CaseAverage = itemStatistics.Where(f => f.ItemNumber.Equals(listItem.ItemNumber) &&
                                                                    f.UnitOfMeasure.Equals("C"))
                                                        .Select(p => p.AverageUse)
                                                        .FirstOrDefault(),
                            PackageAverage = itemStatistics.Where(f => f.ItemNumber.Equals(listItem.ItemNumber) &&
                                                                       f.UnitOfMeasure.Equals("P"))
                                                           .Select(p => p.AverageUse)
                                                           .FirstOrDefault()
                        };
                        listItem.Category = AddContractInformationIfInContract(_contractdictionary, listItem);
                    }
                }
            });

            MarkFavoritesAndAddNotes(user, list, catalogInfo);
        }

        private Dictionary<long, CustomInventoryItem> GetCustomItemHash(UserSelectedContext catalogInfo) {
            var custominv = _customInventoryRepo.GetItemsByBranchAndCustomer(catalogInfo.BranchId, catalogInfo.CustomerId);
            Dictionary<long, CustomInventoryItem> customItemHash = null;
            if (custominv != null)
                customItemHash = custominv.ToDictionary(i => i.Id);
            return customItemHash;
        }

        private void LookupPrices(UserProfile user, List<ListItemModel> listItems, UserSelectedContext catalogInfo)
        {
            if (listItems == null || listItems.Count == 0 || user == null)
                return;

            var prices = new PriceReturn() { Prices = new List<Price>() };

            if (_priceLogic != null) {
                prices.AddRange(_priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1),
                                                      listItems.Where(x => x.CustomInventoryItemId < 1).GroupBy(g => g.ItemNumber)
                                                               .Select(i => new Product()
                                                               {
                                                                   ItemNumber = i.First().ItemNumber,
                                                                   CatchWeight = i.First().CatchWeight,
                                                                   PackagePriceNumeric = i.First().PackagePriceNumeric,
                                                                   CasePriceNumeric = i.First().CasePriceNumeric,
                                                                   CategoryName = i.First().CategoryName,
                                                                   CatalogId = i.First().CatalogId,
                                                                   Unfi = i.First().Unfi
                                                               })
                                                               .Distinct()
                                                               .ToList()
                                                      )
                               );
            }

            Dictionary<string, Price> priceHash = prices.Prices.ToDictionary(p => p.ItemNumber);

            Parallel.ForEach(listItems, listItem =>
            {
                var price = priceHash.ContainsKey(listItem.ItemNumber) ? priceHash[listItem.ItemNumber] : null;
                if (price != null && (listItem.ProprietaryCustomers == null |
                                      (listItem.ProprietaryCustomers != null &&
                                       listItem.ProprietaryCustomers.IndexOf(catalogInfo.CustomerId) > -1)))
                {
                    listItem.PackagePrice = price.PackagePrice.ToString();
                    listItem.CasePrice = price.CasePrice.ToString();
                    if (listItem.CasePrice.Equals("0") & listItem.PackagePrice.Equals("0"))
                    {
                        listItem.IsValid = false;
                    }
                    listItem.DeviatedCost = price.DeviatedCost ? "Y" : "N";
                }
            });
        }

        public List<RecommendedItemModel> ReadRecommendedItemsList(UserSelectedContext catalogInfo) {
            ListModel list = _recommendedItemsLogic.ReadList(new UserProfile(), catalogInfo, false);

            if (list != null &&
                list.Items != null &&
                list.Items.Count > 0) {
                var recommended = list.Items.Select(i => new RecommendedItemModel()
                {
                    ItemNumber = i.ItemNumber,
                    Name = i.Name,
                    Images = (_productImageRepo.GetImageList(i.ItemNumber, true) != null)?
                        _productImageRepo.GetImageList(i.ItemNumber, true).ProductImages:null
                }).ToList();
                
            }
            return new List<RecommendedItemModel>();
        }

        public ListModel MarkFavoritesAndAddNotes(UserProfile user, ListModel list, UserSelectedContext catalogInfo)
        {
            if (list.Items == null || list.Items.Count == 0)
                return null;

            Dictionary<string, ListItemModel> notesHash = GetNotesHash(catalogInfo);

            Dictionary<string, ListItemModel> favHash = GetFavoritesHash(user, catalogInfo);

            Parallel.ForEach(list.Items, listItem =>
            {
                listItem.Favorite = favHash.ContainsKey(listItem.ItemNumber);
                listItem.Notes = notesHash.ContainsKey(listItem.ItemNumber) ? notesHash[listItem.ItemNumber].Notes : null;
            });

            return list;
        }

        public List<Product> MarkFavoritesAndAddNotes(UserProfile user, List<Product> list, UserSelectedContext catalogInfo)
        {
            if (list == null || list.Count == 0)
                return null;

            Dictionary<string, ListItemModel> notesHash = GetNotesHash(catalogInfo);

            Dictionary<string, ListItemModel> favHash = GetFavoritesHash(user, catalogInfo);

            var history = _historyListLogic.ItemsInHistoryList(catalogInfo, list.Select(p => p.ItemNumber).ToList());

            Parallel.ForEach(list, prod =>
            {
                prod.Favorite = favHash.ContainsKey(prod.ItemNumber);
                prod.Notes = notesHash.ContainsKey(prod.ItemNumber) ? notesHash[prod.ItemNumber].Notes : null;
                if (history != null) {
                    prod.InHistory = history.Where(h => h.ItemNumber.Equals(prod.ItemNumber))
                                            .FirstOrDefault()
                                            .InHistory;
                }
            });

            return list;
        }

        private Dictionary<string, ListItemModel> GetFavoritesHash(UserProfile user, UserSelectedContext catalogInfo) {
            ListModel favorites = _favoritesLogic.GetFavoritesList(user.UserId, catalogInfo, false);
            var favHash = new Dictionary<string, ListItemModel>();
            if (favorites != null &&
                favorites.Items != null)
                favHash = favorites.Items
                                   .GroupBy(i => i.ItemNumber)
                                   .ToDictionary(f => f.Key, f => f.First());
            return favHash;
        }

        private Dictionary<string, ListItemModel> GetNotesHash(UserSelectedContext catalogInfo) {
            ListModel notes = _notesLogic.GetList(catalogInfo);
            var notesHash = new Dictionary<string, ListItemModel>();
            if (notes != null &&
                notes.Items != null)
                notesHash = notes.Items
                                 .GroupBy(i => i.ItemNumber)
                                 .ToDictionary(n => n.Key, n => n.First());
            return notesHash;
        }

        private string AddContractInformationIfInContract(Dictionary<string, string> contractdictionary, ListItemModel item)
        {
            return AddContractInformationIfInContract(contractdictionary, item.ItemNumber);
        }

        private string AddContractInformationIfInContract
            (Dictionary<string, string> contractdictionary, string itemNumber)
        {
            string itmcategory = null;
            if (contractdictionary != null && contractdictionary.Count > 0)
            {
                if (contractdictionary.ContainsKey(itemNumber))
                {
                    itmcategory = contractdictionary[itemNumber];
                }
            }

            return itmcategory;
        }



        public List<ItemBarcodeModel> GetBarcodeForList(UserProfile user, UserSelectedContext catalogInfo, ListType type, long listId)
        {
            ListModel list = ReadList(user, catalogInfo, type, listId);

            return _barcodeImageRepo.GetBarcodeForList(list);
        }

        public long? AddCustomInventory(UserProfile user, UserSelectedContext catalogInfo, ListType type, long listId, long customInventoryItemId)
        {
            var list = ReadListById(user, catalogInfo, listId, type);
            CustomInventoryItem customInventoryItem = _customInventoryRepo.Get(customInventoryItemId);

            int position = SetPosition(ref list);

            // If the list type is favorite or reminder, don't allow duplicates
            if (list.Type == ListType.Favorite || list.Type == ListType.Reminder)
            {
                ListItemModel duplicateItem = list.Items.Where(x => x.CustomInventoryItemId.Equals(customInventoryItem.Id)).FirstOrDefault();
                if (duplicateItem != null)
                {
                    return duplicateItem.ListItemId;
                }
            }

            if (customInventoryItem != null) {
                ListItemModel item = new ListItemModel()
                {
                    ItemNumber = customInventoryItem.ItemNumber.Trim(),
                    CatalogId = Constants.CATALOG_CUSTOMINVENTORY,
                    CustomInventoryItemId = customInventoryItemId

                };

                list.Items.Add(item);
                UpdateList(user, catalogInfo, list.Type, list);

                return item.ListItemId;
            }
            return null;
        }

        public List<long?> AddCustomInventoryItems(UserProfile user, UserSelectedContext catalogInfo, ListType type, long listId, List<long> customInventoryItemIds)
        {
            List<long?> returnValue = new List<long?>();
            var list = ReadListById(user, catalogInfo, listId, type);
            List<CustomInventoryItem> customInventoryItems = _customInventoryRepo.GetItemsByItemIds(customInventoryItemIds);

            if (customInventoryItems == null) {
                customInventoryItems = new List<CustomInventoryItem>();
            }

            int position = SetPosition(ref list);

            // If the list type is favorite or reminder, don't allow duplicates
            if (list.Type == ListType.Favorite || list.Type == ListType.Reminder)
            {
                foreach (CustomInventoryItem item in customInventoryItems) {
                    ListItemModel duplicateItem = list.Items.Where(x => x.CustomInventoryItemId.Equals(item.Id)).FirstOrDefault();
                    if (duplicateItem != null)
                    {
                        returnValue.Add(duplicateItem.CustomInventoryItemId);
                    }
                }

                return returnValue;
            }

            foreach (CustomInventoryItem item in customInventoryItems)
            {
                ListItemModel listItem = new ListItemModel();
                listItem.ItemNumber = item.ItemNumber;
                listItem.CatalogId = Constants.CATALOG_CUSTOMINVENTORY;
                listItem.CustomInventoryItemId = item.Id;
                listItem.Position = position++;

                list.Items.Add(listItem);
            }

            UpdateList(user, catalogInfo, type, list);

            foreach (ListItemModel item in list.Items)
            {
                returnValue.Add(item.ListItemId);
            }

            return returnValue;
        }

        private int SetPosition(ref ListModel list)
        {
            int position = 1;

            if (list.Items == null)
            {
                list.Items = new List<ListItemModel>();
            }
            else if (list.Items.Any())
            {
                position = list.Items.Max(i => i.Position) + 1;
            }

            return position;
        }

        public void DeleteItem(UserProfile user, UserSelectedContext catalogInfo, ListType type, 
                               long headerId, string itemNumber) {
            ListModel list      = ReadListById(user, catalogInfo, headerId, type);
            ListItemModel item  = list?.Items
                                      .FirstOrDefault(i => i.ItemNumber == itemNumber);

            if(item != null) {
                item.Active = false;

                SaveItem(user, catalogInfo, type, headerId, item);
            }
        }

        public void DeleteItems(UserProfile user, UserSelectedContext catalogInfo, ListType type, 
                                long headerId, List<string> itemNumbers) {
            ListModel list                      = ReadListById(user, catalogInfo, headerId, type);
            IEnumerable<ListItemModel> items    = list.Items
                                                      .Where(i => itemNumbers.Contains(i.ItemNumber));

            if(items != null) {
                Parallel.ForEach(items, i => {
                                            i.Active = false;
                                        });
                
                SaveItems(user, catalogInfo, type, headerId, 
                          items.ToList());
            }
        }
        #endregion
    }
}