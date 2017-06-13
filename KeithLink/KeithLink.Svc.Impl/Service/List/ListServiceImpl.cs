using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Lists;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Core.Models.Lists.Favorites;
using KeithLink.Svc.Impl.Helpers;

namespace KeithLink.Svc.Impl.Service.List
{
    public class ListServiceImpl : IListService
    {
        #region attributes
        private readonly IListLogic _genericListLogic;
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
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        public ListServiceImpl( IListLogic genericListLogic, IListRepository listRepo, IHistoryListLogic historyListLogic, ICatalogLogic catalogLogic, INotesListLogic notesLogic,
                                IItemHistoryRepository itemHistoryRepo, IFavoritesListLogic favoritesLogic, IPriceLogic priceLogic,
                                IRecentlyViewedListLogic recentlyViewedLogic, IRecentlyOrderedListLogic recentlyOrderedLogic, 
                                IRecommendedItemsListLogic recommendedItemsLogic, IRemindersListLogic reminderItemsLogic,
                                IProductImageRepository productImageRepo, IExternalCatalogRepository externalCatalogRepo,
                                IMandatoryItemsListLogic mandatoryItemsLogic, IInventoryValuationListLogic inventoryValuationLogic,
                                IContractListLogic contractListLogic, ICustomListLogic customListLogic, 
                                IEventLogRepository log)
        {
            _genericListLogic = genericListLogic;
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
            _log = log;
        }
        #endregion

        public List<ListModel> ReadListByType(UserProfile user, UserSelectedContext catalogInfo, ListType type, bool headerOnly = false)
        {
            List<ListModel> returnList = new List<ListModel>();

            switch (type)
            {
                case ListType.Worksheet:
                {
                    returnList.Add(_historyListLogic.GetListModel(user, catalogInfo, 0));

                    FillOutProducts(user, catalogInfo, returnList, true);
                }
                    break;
                default: // plug in legacy list implementation
                    returnList = _genericListLogic.ReadListByType(user, catalogInfo, type, headerOnly);
                    break;
            }

            return returnList;
        }

        public List<ListModel> ReadUserList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly = false)
        {
            List<ListModel> list = new List<ListModel>();

            AddList(user, catalogInfo, headerOnly, list, ListType.Worksheet);
            AddList(user, catalogInfo, headerOnly, list, ListType.Contract);
            AddList(user, catalogInfo, headerOnly, list, ListType.Favorite);
            AddList(user, catalogInfo, headerOnly, list, ListType.Reminder);
            AddList(user, catalogInfo, headerOnly, list, ListType.RecommendedItems);
            AddList(user, catalogInfo, headerOnly, list, ListType.Mandatory);
            AddCustomLists(user, catalogInfo, headerOnly, list);

            // Add a favorite
            //_favoritesLogic.AddOrUpdateFavorite(user, catalogInfo, "025026", false, catalogInfo.BranchId, true);

            // Add a recently viewed
            //_recentlyViewedLogic.AddOrUpdateRecentlyViewed(user, catalogInfo, "987678", false, catalogInfo.BranchId, true);

            // Empty recently viewed
            //_recentlyViewedLogic.DeleteRecentlyViewed(user, catalogInfo);

            // read recently viewed
            //var recentlyViewed = ReadRecent(user, catalogInfo);

            // Add a recently Ordered
            //_recentlyOrderedLogic.AddOrUpdateRecentlyOrdered(user, catalogInfo, "987678", false, catalogInfo.BranchId, true);

            // Empty recently Ordered
            //_recentlyOrderedLogic.DeleteRecentlyOrdered(user, catalogInfo);

            // read recently Ordered
            //var recentlyOrdered = ReadRecentOrder(user, catalogInfo, catalogInfo.BranchId);

            // Add a recommended Items
            //_recommendedItemsLogic.AddOrUpdateRecommendedItem(catalogInfo, "987678", false, catalogInfo.BranchId, true);

            // read recommended Items
            //var recommendedItems = _recommendedItemsLogic.GetRecommendedItemNumbers(user, catalogInfo);

            // Add a reminder Items
            //_reminderItemsLogic.AddOrUpdateReminder(catalogInfo, "987678", false, catalogInfo.BranchId, true);

            // Add a mandatory Items
            //_mandatoryItemsLogic.AddOrUpdateMandatoryItem(catalogInfo, "987678", false, catalogInfo.BranchId, true);

            // Add a note
            //_notesLogic.AddOrUpdateNote(catalogInfo, "082082", true, catalogInfo.BranchId, "There can be only one", true);

            // read notes
            //var notes = _notesLogic.GetNotesDictionary(user, catalogInfo);            

            if (headerOnly)
                return list.Select(l => new ListModel()
                {
                    ListId = l.ListId,
                    Name = l.Name,
                    IsContractList = l.Type == ListType.Contract,
                    IsFavorite = l.Type == ListType.Favorite,
                    IsWorksheet = l.Type == ListType.Worksheet,
                    IsReminder = l.Type == ListType.Reminder,
                    IsMandatory = l.Type == ListType.Mandatory,
                    IsRecommended = l.Type == ListType.RecommendedItems,
                    ReadOnly = l.ReadOnly || (!user.IsInternalUser && l.Type.Equals(ListType.RecommendedItems) || (!user.IsInternalUser && l.Type.Equals(ListType.Mandatory))),
                    SharedWith = l.SharedWith,
                    IsSharing = l.IsSharing,
                    IsShared = l.IsShared,
                    //IsCustomInventory = l.Type == ListType.CustomInventory,
                    Type = l.Type
                })
                           .ToList();

            return list;
        }

        public List<string> ReadLabels(UserProfile user, UserSelectedContext catalogInfo)
        {
            List<ListModel> list = new List<ListModel>();

            AddList(user, catalogInfo, false, list, ListType.Favorite);
            AddCustomLists(user, catalogInfo, false, list);

            //List<ListItem> items = list.Select(l => l.Items).ToList();
            //List<string> labels = ;

            return new List<string>();
        }

        public ListModel ReadList(UserProfile user, UserSelectedContext catalogInfo, ListType type, long Id, bool includePrice = true)
        {
            return ReadListByType(user, catalogInfo, Id, type);
        }

        public PagedListModel ReadPagedList(UserProfile user,
                                            UserSelectedContext catalogInfo,
                                            ListType type,
                                            long Id,
                                            Core.Models.Paging.PagingModel paging)
        {
            System.Diagnostics.Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(gettiming: false);
            ListModel returnList = null;

            returnList = ReadListByType(user, catalogInfo, Id, type);
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
            var list = _recentlyViewedLogic.ReadList(user, catalogInfo, false);

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
            List<ListModel> recentOrders = _recentlyOrderedLogic.ReadList(user,
                new UserSelectedContext() {CustomerId = catalogInfo.CustomerId, BranchId = catalog}, false);

            // Identify specific warehouse - needed for product lookup
            Dictionary<string, string> externalCatalogDict =
                _externalCatalogRepo.ReadAll().ToDictionary(e => e.BekBranchId.ToLower(), e => e.ExternalBranchId);

            List<RecentNonBEKItem> returnItems = recentOrders.SelectMany(i => i.Items
                .Select(l => new RecentNonBEKItem()
                {
                    ItemNumber = l.ItemNumber,
                    CatalogId = externalCatalogDict[catalogInfo.BranchId.ToLower()],
                    ModifiedOn = l.ModifiedUtc
                }))
                .ToList();

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

            return new RecentNonBEKList() { Catalog = catalogInfo.BranchId, Items = returnItems };
        }

        public void SaveItem(UserProfile user, UserSelectedContext catalogInfo, ListType type,
                             long headerId, ListItemModel item) {
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
                    //tempList = _recommendedItemsLogic.GetListModel(user, catalogInfo, Id);
                    break;
                case ListType.Mandatory:
                    //tempList = _mandatoryItemsLogic.GetListModel(user, catalogInfo, Id);
                    break;
                case ListType.Custom:
                    _customListLogic.SaveItem(user, catalogInfo, headerId, item.ToCustomListDetail(headerId));
                    break;
                case ListType.Recent:
                    break;
                case ListType.Notes:
                    break;
                case ListType.InventoryValuation:
                    _inventoryValuationLogic.SaveItem(user, catalogInfo, headerId, item.ToInventoryValuationListDetail(headerId));
                    break;
                case ListType.RecentOrderedNonBEK:
                    break;
                case ListType.CustomInventory:
                    break;
            }
        }

        private ListModel ReadListByType(UserProfile user, UserSelectedContext catalogInfo, long Id, ListType type)
        {
            ListModel tempList = null;
            switch (type)
            {
                case ListType.Worksheet:
                    tempList = _historyListLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.Contract:
                    tempList = _contractListLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.Favorite:
                    tempList = _favoritesLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.Reminder:
                    tempList = _reminderItemsLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.RecommendedItems:
                    tempList = _recommendedItemsLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.Mandatory:
                    tempList = _mandatoryItemsLogic.GetListModel(user, catalogInfo, Id);
                    break;

                case ListType.Custom:
                    tempList = _customListLogic.GetListModel(user, catalogInfo, Id);
                    break;
            }

            if (tempList != null && tempList.Items != null && tempList.Items.Count > 0)
            {
                FillOutProducts(user, catalogInfo, new List<ListModel>() { tempList }, true);
            }

            return tempList;
        }

        private void AddList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly,
                             List<ListModel> list, ListType type) {
            List<ListModel> tempList = new List<ListModel>();

            switch (type) {
                case ListType.Worksheet:
                    tempList.Add(_historyListLogic.GetListModel(user, catalogInfo, 0));
                    break;
                case ListType.Contract:
                    tempList.Add(_contractListLogic.GetListModel(user, catalogInfo, 0));
                    break;
                case ListType.Favorite:
                    tempList.Add(_favoritesLogic.GetListModel(user, catalogInfo, 0));
                    break;
                case ListType.Reminder:
                    tempList.Add(_reminderItemsLogic.GetListModel(user, catalogInfo, 0));
                    break;
                case ListType.RecommendedItems:
                    tempList.Add(_recommendedItemsLogic.GetListModel(user, catalogInfo, 0));
                    break;
                case ListType.Mandatory:
                    tempList.Add(_mandatoryItemsLogic.GetListModel(user, catalogInfo, 0));
                    break;
            }

            if(tempList.Count > 0 && 
               tempList[0].Items != null && 
               tempList[0].Items.Count > 0) {
                FillOutProducts(user, catalogInfo, tempList, true);
            }

            if (tempList.Count > 0) {
                list.AddRange(tempList);
            }
        }

        private void AddCustomLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly,
            List<ListModel> list)
        {
            List<ListModel> tempList = _customListLogic.ReadList(user, catalogInfo, true);

            if (tempList != null && tempList.Count > 0 && tempList[0].Items != null && tempList[0].Items.Count > 0)
            {
                foreach (ListModel tlist in tempList)
                {
                    FillOutProducts(user, catalogInfo, new List<ListModel>() { tlist }, true);
                }
            }

            if (tempList != null)
            {
                list.AddRange(tempList);
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

        private void AddOtherLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly,
            List<ListModel> list)
        {
            List<ListModel> theRest = _genericListLogic.ReadUserList(user, catalogInfo, headerOnly);
            list.AddRange(theRest.Where(l => l.Type != ListType.Worksheet && l.Type != ListType.Favorite));
            //except what we already define in the specific lists (add other types we define)
        }

        private void FillOutProducts(UserProfile user, UserSelectedContext catalogInfo, List<ListModel> returnList, bool getprices)
        {
//Lookup product details for each item
            returnList.ForEach(delegate(ListModel listItem) { LookupProductDetails(user, listItem, catalogInfo); });

            if (getprices)
            {
                //Lookup prices for each item
                foreach (var tempList in returnList)
                {
                    LookupPrices(user, tempList.Items, catalogInfo);
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
            ProductsReturn products = new ProductsReturn() { Products = new List<Product>() };

            {
                var batch = list.Items.Select(i => i.ItemNumber)
                                      .ToList();

                var tempProducts = _catalogLogic.GetProductsByIds(catalogInfo.BranchId, batch);

                products.Products.AddRange(tempProducts.Products);
            }

            var productHash = products.Products.GroupBy(p => p.ItemNumber)
                                               .Select(i => i.First())
                                               .ToDictionary(p => p.ItemNumber);
            List<ItemHistory> itemStatistics = _itemHistoryRepo.Read(f => f.BranchId.Equals(catalogInfo.BranchId, StringComparison.InvariantCultureIgnoreCase) &&
                                                                          f.CustomerNumber.Equals(catalogInfo.CustomerId))
                                                               .ToList();
            Parallel.ForEach(list.Items, listItem =>
            {
                    var prod = productHash.ContainsKey(listItem.ItemNumber) ? productHash[listItem.ItemNumber] : null;

                    if (prod != null)
                    {
                        listItem.IsValid = true;
                        listItem.Name = prod.Name;
                        listItem.Pack = prod.Pack;
                        listItem.Size = prod.Size;
                        listItem.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
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
                }
            });
        }

        private void LookupPrices(UserProfile user, List<ListItemModel> listItems, UserSelectedContext catalogInfo)
        {
            if (listItems == null || listItems.Count == 0 || user == null)
                return;

            var prices = new PriceReturn() { Prices = new List<Price>() };

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
                    listItem.DeviatedCost = price.DeviatedCost ? "Y" : "N";
                }
            });
        }

    }
}
