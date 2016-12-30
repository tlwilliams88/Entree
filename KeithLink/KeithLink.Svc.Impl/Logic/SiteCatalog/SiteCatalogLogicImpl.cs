// KeithLink
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Models.Configuration.EF;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.ModelExport;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;

using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.History;

using KeithLink.Svc.Core.Extensions;

// Core
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Helpers;

namespace KeithLink.Svc.Impl.Logic.SiteCatalog
{
    public class SiteCatalogLogicImpl : ICatalogLogic
    {
        #region attributes
        private readonly ICacheRepository _catalogCacheRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly ICategoryImageRepository _categoryImageRepository;
		private readonly IDivisionLogic _divisionLogic;
        private readonly IExportSettingLogic _externalCatalogRepository;
        private readonly IFavoriteLogic _favoriteLogic;
        private readonly IHistoryLogic _historyLogic;
        private readonly IListRepository _listRepo;
        private readonly IProductImageRepository _imgRepository;
        private readonly IOrderHistoryDetailRepository _orderDetailRepo;
        private readonly IOrderHistoryHeaderRepsitory _orderHeaderRepo;
        private readonly INoteLogic _noteLogic;
        private readonly IPriceLogic _priceLogic;

        protected string CACHE_GROUPNAME { get { return "Catalog"; } }
        protected string CACHE_NAME { get { return "Catalog"; } }
		protected string CACHE_PREFIX { get { return "Default"; } }
        #endregion

        #region constructor
        public SiteCatalogLogicImpl(ICatalogRepository catalogRepository, IPriceLogic priceLogic, IProductImageRepository imgRepository, ICategoryImageRepository categoryImageRepository, 
                                    ICacheRepository catalogCacheRepository, IDivisionLogic divisionLogic, IOrderHistoryHeaderRepsitory orderHistoryHeaderRepo, 
                                    IOrderHistoryDetailRepository orderHistoryDetailRepo, IExportSettingLogic externalCatalogRepository, IFavoriteLogic favoriteLogic, 
                                    INoteLogic noteLogic, IHistoryLogic historyLogic, IListRepository listRepo)
        {
            _catalogCacheRepository = catalogCacheRepository;
            _catalogRepository = catalogRepository;
            _categoryImageRepository = categoryImageRepository;
            _divisionLogic = divisionLogic;
            _externalCatalogRepository = externalCatalogRepository;
            _favoriteLogic = favoriteLogic;
            _historyLogic = historyLogic;
            _imgRepository = imgRepository;
            _orderDetailRepo = orderHistoryDetailRepo;
            _orderHeaderRepo = orderHistoryHeaderRepo;
            _noteLogic = noteLogic;
            _listRepo = listRepo;
            _priceLogic = priceLogic;
        }
        #endregion

        #region methods
        private void AddCategoryImages(CategoriesReturn returnValue) {
            foreach (Category c in returnValue.Categories) {
                c.CategoryImage = _categoryImageRepository.GetImageByCategory(c.Id).CategoryImage;
            }
        }

        private void AddCategorySearchName(CategoriesReturn returnValue) {
            foreach (Category c in returnValue.Categories) {
                c.SearchName = GetCategorySearchName(c.Name);
                foreach (SubCategory sc in c.SubCategories)
                    sc.SearchName = GetCategorySearchName(sc.Name);
            }
        }

        private void AddItemHistoryToProduct(Product returnValue, UserSelectedContext catalogInfo) {
            List<OrderHistoryFile> history = GetLastFiveOrderHistory(catalogInfo, returnValue.ItemNumber);

            foreach (OrderHistoryFile h in history) {
                foreach (OrderHistoryDetail d in h.Details) {
                    if (returnValue.OrderHistory.ContainsKey(h.Header.DeliveryDate))
                        returnValue.OrderHistory[h.Header.DeliveryDate] += d.ShippedQuantity;
                    else
                        returnValue.OrderHistory.Add(h.Header.DeliveryDate, d.ShippedQuantity);
                }
            }
        }

        public void AddPricingInfo(ProductsReturn prods, UserSelectedContext context, SearchInputModel searchModel) {
            if (context == null || String.IsNullOrEmpty(context.CustomerId))
                return;

            PriceReturn pricingInfo = null;
            if (prods.Products.Count > 0 && IsSpecialtyCatalog(null, prods.Products[0].CatalogId))
            {
                string source = GetCatalogTypeFromCatalogId(prods.Products[0].CatalogId);
                pricingInfo = _priceLogic.GetNonBekItemPrices("fdf", context.CustomerId, source, DateTime.Now.AddDays(1), prods.Products);
            } else {
                pricingInfo = _priceLogic.GetPrices(context.BranchId, context.CustomerId, DateTime.Now.AddDays(1), prods.Products);
            }

            foreach (Price p in pricingInfo.Prices) {
                Product prod = prods.Products.Find(x => x.ItemNumber == p.ItemNumber);

                    prod.CasePrice = p.CasePrice.ToString();
                    prod.CasePriceNumeric = p.CasePrice;
                    prod.PackagePrice = p.PackagePrice.ToString();
                    prod.PackagePriceNumeric = p.PackagePrice;
                    prod.DeviatedCost = p.DeviatedCost ? "Y" : "N";
                //}
            }

            if ((searchModel.SField == "caseprice" || searchModel.SField == "unitprice") && prods.TotalCount <= Configuration.MaxSortByPriceItemCount) // sort pricing info first
            {
                if (searchModel.SDir == "asc")
                    if (searchModel.SField == "caseprice")
                        prods.Products.Sort((x, y) => x.CasePriceNumeric.CompareTo(y.CasePriceNumeric));
                    else
                        prods.Products.Sort((x, y) => x.UnitCost.CompareTo(y.UnitCost));
                else
                    if (searchModel.SField == "caseprice")
                    prods.Products.Sort((x, y) => y.CasePriceNumeric.CompareTo(x.CasePriceNumeric));
                else
                    prods.Products.Sort((x, y) => y.UnitCost.CompareTo(x.UnitCost));
            }
        }

        private void AddProductImageInfo(Product ret)
        {
            //ret.ProductImages = _imgRepository.GetImageList(ret.ItemNumber).ProductImages;
            if (ret.Unfi != null) {
                ret.ProductImages = _imgRepository.GetImageList(ret.UPC, false).ProductImages;
            } else {
                ret.ProductImages = _imgRepository.GetImageList(ret.ItemNumber).ProductImages;
            }
        }

        private void GetAdditionalProductInfo(UserProfile profile, ProductsReturn ret, UserSelectedContext catalogInfo) {
            if (profile != null) {
                var favorites = _favoriteLogic.GetFavoritedItemNumbers(profile, catalogInfo);
                var notes = _noteLogic.GetNotes(profile, catalogInfo);
                var history = _historyLogic.ItemsInHistoryList(catalogInfo, ret.Products.Select(p => p.ItemNumber).ToList());

                ret.Products.ForEach(delegate(Product prod) {
                    prod.Favorite = favorites.Contains(prod.ItemNumber);
                    prod.Notes = notes.Where(n => n.ItemNumber.Equals(prod.ItemNumber))
                                      .Select(i => i.Notes)
                                      .FirstOrDefault();
                    prod.InHistory = history.Where(h => h.ItemNumber.Equals(prod.ItemNumber))
                                            .FirstOrDefault()
                                            .InHistory;
                    AddProductImageInfo(prod);
                });
            }
        }

        public string GetBranchId(string bekBranchId, string catalogType) {
            if (catalogType.ToLower() != "bek") {
                //Go get the code for this branch, hard code for now
                //filteredList= listOfThings.Where(x => x.BranchId == "FOK");
                List<ExportExternalCatalog> externalCatalog = _externalCatalogRepository.ReadExternalCatalogs()
                                                                                        .Where(x => x.Type.Equals(catalogType, StringComparison.InvariantCultureIgnoreCase))
                                                                                        .ToList();

                List<ExportExternalCatalog> filteredList = externalCatalog.Where(x => bekBranchId.Equals(x.BekBranchId.Trim(), StringComparison.InvariantCultureIgnoreCase)).ToList();


                if (filteredList.Count > 0) {
                    return filteredList[0].CatalogId;
                } else {
                    return bekBranchId;
                }
            } else {
                return bekBranchId;
            }
        }

        public string GetCatalogTypeFromCatalogId(string catalogId) {
            //Go get the code for this branch, hard code for now
            //filteredList= listOfThings.Where(x => x.BranchId == "FOK");
            List<ExportExternalCatalog> externalCatalog = _externalCatalogRepository.ReadExternalCatalogs()
                                                                                    .Where(x => catalogId.Equals(x.CatalogId, StringComparison.InvariantCultureIgnoreCase))
                                                                                    .ToList();

            if (externalCatalog.Count > 0) {
                if (externalCatalog[0].Type != null) {
                    return externalCatalog[0].Type.ToString();
                } else {
                    return catalogId;
                }
            } else {
                return "BEK";
            }
        }

        public CategoriesReturn GetCategories(int from, int size, string catalogType)
        {
            CategoriesReturn categoriesReturn = _catalogCacheRepository.GetItem<CategoriesReturn>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCategoriesCacheKey(from, size, catalogType));
            if (categoriesReturn == null) {
                categoriesReturn = _catalogRepository.GetCategories(from, size, catalogType);
                AddCategoryImages(categoriesReturn);
                AddCategorySearchName(categoriesReturn);
                _catalogCacheRepository.AddItem<CategoriesReturn>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, GetCategoriesCacheKey(from, size, catalogType), TimeSpan.FromHours(2), categoriesReturn);
            }
            return categoriesReturn;
        }

        private static string GetCategoriesCacheKey(int from, int size, string catalogType) {
            return String.Format("CategoriesReturn_{0}_{1}_{2}", from, size, catalogType);
        }

        private string GetCategorySearchName(string categoryName) {
            // remove ',' and '.', replace '&' with 'and', replace white space and / with _, lowercase
            if (!String.IsNullOrEmpty(categoryName))
                return categoryName.Replace("&", "and").Replace(",", "").Replace(" ", "_").Replace("/", "_").Replace(".", "").ToLower();
            return categoryName;
        }

        public List<Division> GetDivisions() {
            return _divisionLogic.GetDivisions();
        }

        public List<String> GetExternalBranchIds(string bekBranchId) {
            return _externalCatalogRepository.ReadExternalCatalogs()
                                             .Where(x => bekBranchId.Equals(x.BekBranchId, StringComparison.InvariantCultureIgnoreCase))
                                             .Select(c => c.CatalogId)
                                             .ToList();
        }

        public Dictionary<string, int> GetHitsForCatalogs(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel) {
            var newSearchModel = new SearchInputModel();
            newSearchModel.CatalogType = searchModel.CatalogType;
            newSearchModel.Dept = searchModel.Dept;
            newSearchModel.Facets = searchModel.Facets;
            newSearchModel.From = searchModel.From;
            newSearchModel.SDir = searchModel.SDir;
            newSearchModel.SField = searchModel.SField;
            newSearchModel.Size = 1; //This will minimize number returned from elastic search to minimize processing for only count

            List<ExportExternalCatalog> externalCatalog = _externalCatalogRepository.ReadExternalCatalogs().ToList();
            var listOfCatalogs = externalCatalog.Select(x => x.Type).Distinct().ToList();
            listOfCatalogs.Add("BEK");

            var baseCatalogTypeIndex = listOfCatalogs.IndexOf(newSearchModel.CatalogType);
            if (baseCatalogTypeIndex != -1)
                listOfCatalogs.RemoveAt(baseCatalogTypeIndex);

            var returnDict = new Dictionary<string, int>();
            foreach (var catalog in listOfCatalogs) {
                var catalogTempInfo = new UserSelectedContext();
                catalogTempInfo.CustomerId = catalogInfo.CustomerId;
                catalogTempInfo.BranchId = GetBranchId(catalogInfo.BranchId, catalog);
                returnDict.Add(catalog.ToLower(), _catalogRepository.GetHitsForSearchInIndex(catalogTempInfo, search, newSearchModel));
            }

            return returnDict;
        }

        public ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel, UserProfile profile) {
            ProductsReturn returnValue;

            // special handling for price sorting
            if (searchModel.SField == "caseprice")
                returnValue = _catalogRepository.GetHouseProductsByBranch(catalogInfo, brandControlLabel, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = Configuration.MaxSortByPriceItemCount });
            else
                returnValue = _catalogRepository.GetHouseProductsByBranch(catalogInfo, brandControlLabel, searchModel);

            AddPricingInfo(returnValue, catalogInfo, searchModel);
            GetAdditionalProductInfo(profile, returnValue, catalogInfo);

            return returnValue;
        }

        private List<OrderHistoryFile> GetLastFiveOrderHistory(UserSelectedContext catalogInfo, string itemNumber) {
            List<OrderHistoryFile> returnValue = new List<OrderHistoryFile>();

            List<EF.OrderHistoryHeader> history = _orderHeaderRepo.GetLastFiveOrdersByItem(catalogInfo.BranchId, catalogInfo.CustomerId, itemNumber);

            foreach(EF.OrderHistoryHeader h in history) {
                OrderHistoryFile root = new OrderHistoryFile() {
                    Header = new OrderHistoryHeader() {
                        BranchId = h.BranchId,
                        CustomerNumber = h.CustomerNumber,
                        InvoiceNumber = h.CustomerNumber,
                        DeliveryDate = h.DeliveryDate,
                        PONumber = h.PONumber,
                        ControlNumber = h.ControlNumber,
                        OrderStatus = h.OrderStatus,
                        FutureItems = h.FutureItems,
                        ErrorStatus = h.ErrorStatus,
                        ActualDeliveryTime = h.ActualDeliveryTime,
                        EstimatedDeliveryTime = h.EstimatedDeliveryTime,
                        ScheduledDeliveryTime = h.ScheduledDeliveryTime,
                        DeliveryOutOfSequence = h.DeliveryOutOfSequence,
                        RouteNumber = h.RouteNumber,
                        StopNumber = h.StopNumber
                    }
                };

                root.Details.AddRange(_orderDetailRepo.Read(d => d.BranchId == h.BranchId &&
                                                                 d.OrderHistoryHeader.Id == h.Id &&
                                                                 d.ItemNumber == itemNumber)
                                                      .Select(x => new OrderHistoryDetail() {
                                                          LineNumber = x.LineNumber,
                                                          ItemNumber = x.ItemNumber,
                                                          OrderQuantity = x.OrderQuantity,
                                                          ShippedQuantity = x.ShippedQuantity
                                                      })
                                                      .ToList());
                returnValue.Add(root);
            }

            return returnValue;
        }

        public Product GetProductById(UserSelectedContext catalogInfo, string id, UserProfile profile, string catalogType) {
            var bekBranchId = catalogInfo.BranchId;
            string catalogId = GetBranchId( catalogInfo.BranchId, catalogType );
            catalogInfo.BranchId = catalogId;  
            Product ret = _catalogRepository.GetProductById(catalogInfo.BranchId, id);
            Dictionary<string, string> contractdictionary = ContractInformationHelper.GetContractInformation(catalogInfo, _listRepo, _catalogCacheRepository);

            if (ret == null)
                return null;

            catalogInfo.BranchId = bekBranchId;
            GetAdditionalProductInfo(profile, new ProductsReturn() { Count = 1, Products = new List<Product>() { ret } }, catalogInfo);
            catalogInfo.BranchId = catalogId;
            //AddFavoriteProductInfo(profile, ret, catalogInfo);
            AddProductImageInfo(ret);
            AddItemHistoryToProduct(ret, catalogInfo);
            ret.Category = ContractInformationHelper.AddContractInformationIfInContract(contractdictionary, new ListItemModel() { ItemNumber = ret.ItemNumber });

            PriceReturn pricingInfo = null;

            //if (IsSpecialtyCatalog(catalogType)) {
            //    pricingInfo = _priceLogic.GetNonBekItemPrices(bekBranchId, catalogInfo.CustomerId, catalogType, DateTime.Now.AddDays(1), new List<Product>() { ret });
            //} else { 
            pricingInfo = _priceLogic.GetPrices(bekBranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), new List<Product>() { ret });
            //}

            if (pricingInfo != null && pricingInfo.Prices.Where(p => p.ItemNumber.Equals(ret.ItemNumber)).Any()) {
                var price = pricingInfo.Prices.Where(p => p.ItemNumber.Equals(ret.ItemNumber)).First();
                ret.CasePrice = price.CasePrice.ToString();
                ret.CasePriceNumeric = price.CasePrice;
                ret.PackagePrice = price.PackagePrice.ToString();
                ret.PackagePriceNumeric = price.PackagePrice;
                ret.DeviatedCost = price.DeviatedCost ? "Y" : "N";
            }

            return ret;
        }

        public Product GetProductByIdOrUPC(UserSelectedContext catalogInfo, string idorupc, UserProfile profile) {
            Product ret = null;
            if (idorupc.Length <= 6)
                ret = _catalogRepository.GetProductById(catalogInfo.BranchId, idorupc);
            else {
                //Try to find by UPC
                ProductsReturn products = GetProductsBySearch(catalogInfo, idorupc, new SearchInputModel() { From = 0, Size = 10, SField = "upc" }, profile);
                foreach (Product p in products.Products) {
                    if (p.UPC == idorupc) {
                        return p;
                    }
                }
            }


            if (ret == null)
                return null;

            GetAdditionalProductInfo(profile, new ProductsReturn() { Count = 1, Products = new List<Product>() { ret } }, catalogInfo);

            //AddFavoriteProductInfo(profile, ret, catalogInfo);
            AddProductImageInfo(ret);
            AddItemHistoryToProduct(ret, catalogInfo);

            //PriceReturn pricingInfo = _priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), new List<Product>() { ret });
            PriceReturn pricingInfo = null;
            //if (!IsSpecialtyCatalog(null,ret.CatalogId))
            pricingInfo = _priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), new List<Product>() { ret });
            //else
            //    pricingInfo = _priceLogic.GetNonBekItemPrices(catalogInfo.BranchId, catalogInfo.CustomerId, GetCatalogTypeFromCatalogId(ret.CatalogId), DateTime.Now.AddDays(1), new List<Product>() { ret });

            if (pricingInfo != null && pricingInfo.Prices.Where(p => p.ItemNumber.Equals(ret.ItemNumber)).Any()) {
                var price = pricingInfo.Prices.Where(p => p.ItemNumber.Equals(ret.ItemNumber)).First();
                ret.CasePrice = price.CasePrice.ToString();
                ret.CasePriceNumeric = price.CasePrice;
                ret.PackagePrice = price.PackagePrice.ToString();
                ret.PackagePriceNumeric = price.PackagePrice;
                ret.DeviatedCost = price.DeviatedCost ? "Y" : "N";
            }

            return ret;
        }

        public ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel, UserProfile profile) {
            ProductsReturn ret;
            string categoryName = category;
           
            // enable category search on either category id or search name
            Category catFromSearchName = this.GetCategories(0, 2000, searchModel.CatalogType).Categories.Where(x => x.SearchName == category).FirstOrDefault();
            if (catFromSearchName != null)
                categoryName = catFromSearchName.Name;

            if (searchModel.CatalogType.Equals(Constants.CATALOG_UNFI, StringComparison.InvariantCultureIgnoreCase))
                categoryName = categoryName.ToUpper();

            var newCatalog = new UserSelectedContext() { CustomerId = catalogInfo.CustomerId, BranchId = GetBranchId(catalogInfo.BranchId, searchModel.CatalogType) };

            // special handling for price sorting
            if (searchModel.SField == "caseprice") // we have to block caseprice from the searchModel used in es
                ret = _catalogRepository.GetProductsByCategory(newCatalog, categoryName, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = searchModel.Size });
            else
                ret = _catalogRepository.GetProductsByCategory(newCatalog, categoryName, searchModel);

            AddPricingInfo(ret, catalogInfo, searchModel);

            GetAdditionalProductInfo(profile, ret, catalogInfo);

            // special handling for price sorting - handle the pricesort
            if (searchModel.SField == "caseprice" && searchModel.SDir == "asc")
                ret.Products = ret.Products.OrderBy(p => p.CasePriceNumeric).ToList();
            else if (searchModel.SField == "caseprice" && searchModel.SDir == "desc")
                ret.Products = ret.Products.OrderByDescending(p => p.CasePriceNumeric).ToList();

            return ret;
        }

        public ProductsReturn GetProductsByIds(string branch, List<string> ids) {
            var products = new ProductsReturn() { Products = new List<Product>() };

            var branches = GetExternalBranchIds(branch);
            branches.Insert(0, branch.ToLower());

            foreach (var catalogId in branches)
            {
                int totalProcessed = 0;
                while (totalProcessed < ids.Count)
                {
                    var tempProducts = _catalogRepository.GetProductsByIds(catalogId, ids.Skip(totalProcessed).Take(500).Distinct().ToList());

                    if (tempProducts != null && tempProducts.Products != null)
                    {
                        products.Count += tempProducts.Count;
                        products.TotalCount += tempProducts.TotalCount;
                        products.Products.AddRange(tempProducts.Products);
                    }

                    totalProcessed += 500;
                }

                foreach (var prod in products.Products.Where(p => p.CatalogId == catalogId))
                {
                    prod.CatalogId = catalogId;
                    ids.Remove(prod.ItemNumber);
                }
            }

            return products;
        }

        public ProductsReturn GetProductsByIdsWithPricing(UserSelectedContext catalogInfo, List<string> ids) {
            int totalProcessed = 0;
            var products = new ProductsReturn() { Products = new List<Product>() };

            while (totalProcessed < ids.Count) {
                var tempProducts = _catalogRepository.GetProductsByIds(catalogInfo.BranchId, ids.Skip(totalProcessed).Take(500).Distinct().ToList());

                if (tempProducts != null && tempProducts.Products != null && tempProducts.Products.Count > 0) {
                    products.Count += tempProducts.Count;
                    products.TotalCount += tempProducts.TotalCount;
                    products.Products.AddRange(tempProducts.Products);
                }

                totalProcessed += 500;
            }

            if (products.Products.Count > 0)
            {
                AddPricingInfo(products, catalogInfo, new SearchInputModel());
            }

            return products;
        }

        public ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel, UserProfile profile) {
            ProductsReturn ret;
            var catalogCounts = GetHitsForCatalogs(catalogInfo, search, searchModel);
            var tempCatalogInfo = new UserSelectedContext();
            tempCatalogInfo.CustomerId = catalogInfo.CustomerId;
            tempCatalogInfo.BranchId = GetBranchId(catalogInfo.BranchId, searchModel.CatalogType);

            // special handling for price sorting
            if (searchModel.SField == "caseprice" || searchModel.SField == "unitprice")
                ret = _catalogRepository.GetProductsBySearch(tempCatalogInfo, 
                                                             search, 
                                                             new SearchInputModel() { 
                                                                Facets = searchModel.Facets, 
                                                                From = searchModel.From, 
                                                                Size = Configuration.MaxSortByPriceItemCount,
                                                                Dept = searchModel.Dept, 
																CatalogType = searchModel.CatalogType
                                                                }
                                                            );
            else
                ret = _catalogRepository.GetProductsBySearch(tempCatalogInfo, search, searchModel);

            AddPricingInfo(ret, catalogInfo, searchModel);
            GetAdditionalProductInfo(profile, ret, catalogInfo);
            ret.CatalogCounts = catalogCounts;

            foreach (var product in ret.Products)
            {
                product.IsSpecialtyCatalog = IsSpecialtyCatalog(searchModel.CatalogType);
            }

            return ret;
        }

        public bool IsCatalogIdBEK(string catalogId)
        {
            List<string> bekBranchIds = _externalCatalogRepository.ReadExternalCatalogs().Select(x => x.BekBranchId).Distinct().ToList();

            return bekBranchIds.Contains(catalogId, StringComparer.InvariantCultureIgnoreCase);
        }

        public bool IsSpecialtyCatalog(string catalogType, string branchId = null)
        {
            if (!String.IsNullOrEmpty(catalogType))
                return !catalogType.Equals("bek", StringComparison.InvariantCultureIgnoreCase);
            else if (!String.IsNullOrEmpty(branchId))
            {
                //look up branch and see if it is BEK
                List<ExportExternalCatalog> externalCatalog = _externalCatalogRepository.ReadExternalCatalogs()
                                                                                        .Where(x => branchId.Equals(x.CatalogId, StringComparison.InvariantCultureIgnoreCase))
                                                                                        .ToList();
                return (externalCatalog.Count > 0);
            }
            else
                return false;
        }
        #endregion
	}
}
