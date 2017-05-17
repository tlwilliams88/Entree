using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Customers.EF;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Models.EF;
using KeithLink.Svc.Impl.Helpers;

namespace KeithLink.Svc.Impl.Service.List
{
    public class ListServiceImpl : IListService
    {
        #region attributes
        private readonly IListLogic _genericListLogic;
        private readonly IListRepository _listRepo;
        private readonly IHistoryListLogic _historyListLogic;
        private readonly ICatalogLogic _catalogLogic;
        private readonly IItemHistoryRepository _itemHistoryRepo;
        private readonly IPriceLogic _priceLogic;
        private readonly IEventLogRepository _log;
        #endregion

        #region ctor
        public ListServiceImpl( IListLogic genericListLogic, IListRepository listRepo, IHistoryListLogic historyListLogic, ICatalogLogic catalogLogic, 
                                IItemHistoryRepository itemHistoryRepo, IPriceLogic priceLogic, IEventLogRepository log)
        {
            _genericListLogic = genericListLogic;
            _listRepo = listRepo;
            // specific lists -
            _historyListLogic = historyListLogic;
            _catalogLogic = catalogLogic;
            _itemHistoryRepo = itemHistoryRepo;
            _priceLogic = priceLogic;
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
                    returnList = _historyListLogic.ReadList(catalogInfo, headerOnly);

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

            AddHistoryList(user, catalogInfo, headerOnly, list);

            AddOtherLists(user, catalogInfo, headerOnly, list);

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
                    IsShared = !l.IsShared,
                    //IsCustomInventory = l.Type == ListType.CustomInventory,
                    Type = l.Type
                })
                           .ToList();

            return list;
        }

        public PagedListModel ReadPagedList(UserProfile user,
                                            UserSelectedContext catalogInfo,
                                            long Id,
                                            Core.Models.Paging.PagingModel paging)
        {
            System.Diagnostics.Stopwatch stopWatch = EntreeStopWatchHelper.GetStopWatch(gettiming: false);
            ListModel returnList = null;

            {
                ListModel historyList = _historyListLogic.GetListModel(user, catalogInfo, Id);
                if (historyList != null && historyList.ListId == Id)
                {
                    FillOutProducts(user, catalogInfo, new List<ListModel>() { historyList }, true);

                    returnList = historyList;
                }
            }

            if (returnList == null)
            {
                List<ListModel> otherLists = _genericListLogic.ReadUserList(user, catalogInfo)
                                                              .Where(l => l.Type != ListType.Worksheet) //except what we already define in the specific lists (add other types we define)
                                                              .ToList();
                foreach (ListModel list in otherLists)
                {
                    if (returnList == null && list.ListId == Id)
                    {
                        returnList = list;
                    }
                }
            }
            stopWatch.Read(_log, "ReadPagedList - GetListModel");

            PagedListModel pagedList = returnList.ToPagedList(paging);
            stopWatch.Read(_log, "ReadPagedList - ToPagedList");

            return pagedList;
        }

        private void AddOtherLists(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly,
            List<ListModel> list)
        {
            List<ListModel> theRest = _genericListLogic.ReadUserList(user, catalogInfo, headerOnly);
            list.AddRange(theRest.Where(l => l.Type != ListType.Worksheet));
            //except what we already define in the specific lists (add other types we define)
        }

        private void AddHistoryList(UserProfile user, UserSelectedContext catalogInfo, bool headerOnly,
            List<ListModel> list)
        {
            List<ListModel> worksheet = _historyListLogic.ReadList(catalogInfo, headerOnly);

            FillOutProducts(user, catalogInfo, worksheet, true);

            list.AddRange(worksheet);
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
