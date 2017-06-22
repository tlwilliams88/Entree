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
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Models.Customers.EF;

namespace KeithLink.Svc.Impl.Service.SiteCatalog
{
    public class SiteCatalogServiceImpl : ISiteCatalogService
    {
        #region attributes
        private readonly ICacheRepository _catalogCacheRepository;
        private readonly ICatalogLogic _catalogLogic;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IPriceLogic _priceLogic;
        private readonly IListRepository _listRepo;
        private readonly IFavoritesListLogic _favoritesLogic;
        private readonly IHistoryLogic _historyLogic;
        private readonly IItemHistoryRepository _itemHistoryRepo;
        private readonly INotesListLogic _notesLogic;

        protected string CACHE_GROUPNAME { get { return "Catalog"; } }
        protected string CACHE_NAME { get { return "Catalog"; } }
        protected string CACHE_PREFIX { get { return "Default"; } }
        #endregion

        #region constructor
        public SiteCatalogServiceImpl(ICacheRepository catalogCacheRepository, ICatalogLogic catalogLogic
                                      , ICatalogRepository catalogRepository, IPriceLogic priceLogic
                                      , IListRepository listRepo, IFavoritesListLogic favoritesLogic
                                      , IHistoryLogic historyLogic, INotesListLogic notesLogic
                                      , IItemHistoryRepository itemHistoryRepo)
        {
            _catalogCacheRepository = catalogCacheRepository;
            _catalogLogic = catalogLogic;
            _catalogRepository = catalogRepository;
            _priceLogic = priceLogic;
            _listRepo = listRepo;
            _favoritesLogic = favoritesLogic;
            _historyLogic = historyLogic;
            _itemHistoryRepo = itemHistoryRepo;
            _notesLogic = notesLogic;
        }
        #endregion

        #region methods
        #region by category
        public ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo,
                                                    string category,
                                                    SearchInputModel searchModel,
                                                    UserProfile profile)
        {
            ProductsReturn ret;
            string categoryName = _catalogLogic.GetCategoryName(category, searchModel);

            var newCatalog = new UserSelectedContext() { CustomerId = catalogInfo.CustomerId,
                                                         BranchId = _catalogLogic.GetBranchId(catalogInfo.BranchId, 
                                                                                              searchModel.CatalogType) };

            List<string> specialFilters = _catalogRepository.SeekSpecialFilters(searchModel.Facets);

            // special handling for price sorting
            if (searchModel.SField == "caseprice") // we have to block caseprice from the searchModel used in es
                ret = _catalogRepository.GetProductsByCategory(newCatalog, categoryName, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = searchModel.Size });
            else if (specialFilters.Count>0)
            {
                ret = GetAllCategoryProductsShallow(searchModel, categoryName, newCatalog);
            }
            else
            {
                ret = _catalogRepository.GetProductsByCategory(newCatalog, categoryName, searchModel);
            }

            ret = ApplySpecialFilters(catalogInfo, profile, specialFilters, searchModel, ret);

            AddPricingInfo(ret, catalogInfo, searchModel);

            GetAdditionalProductInfo(profile, ret, catalogInfo);

            return ret;
        }

        private ProductsReturn GetAllCategoryProductsShallow(SearchInputModel searchModel, string categoryName,
            UserSelectedContext newCatalog)
        {
            // get just the itemnumber and catalogid of the products matching the query
            searchModel.Size = 1; // set size 1 to to min results; we want total
            ProductsReturn ret = _catalogRepository.GetProductNumbersByCategory(newCatalog, categoryName, searchModel);

            searchModel.Size = ret.TotalCount; // set size total from before
            ret = _catalogRepository.GetProductNumbersByCategory(newCatalog, categoryName, searchModel);
            return ret;
        }
        #endregion

        #region by search
        public ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo, 
                                                  string search, 
                                                  SearchInputModel searchModel, 
                                                  UserProfile profile)
        {
            ProductsReturn ret;
            var catalogCounts = _catalogLogic.GetHitsForCatalogs(catalogInfo, search, searchModel);
            var tempCatalogInfo = new UserSelectedContext();
            tempCatalogInfo.CustomerId = catalogInfo.CustomerId;
            tempCatalogInfo.BranchId = _catalogLogic.GetBranchId(catalogInfo.BranchId, searchModel.CatalogType);

            List<string> specialFilters = _catalogRepository.SeekSpecialFilters(searchModel.Facets);

            // special handling for price sorting
            if (searchModel.SField == "caseprice" || searchModel.SField == "unitprice")
            {
                ret = _catalogRepository.GetProductsBySearch(tempCatalogInfo,
                                                             search,
                                                             new SearchInputModel()
                                                             {
                                                                 Facets = searchModel.Facets,
                                                                 From = searchModel.From,
                                                                 Size = Configuration.MaxSortByPriceItemCount,
                                                                 Dept = searchModel.Dept,
                                                                 CatalogType = searchModel.CatalogType
                                                             }
                                                            );
            }
            else if (specialFilters.Count>0)
            {
                ret = GetAllProductsBySearchShallow(search, searchModel, tempCatalogInfo);
            }
            else
            {
                ret = _catalogRepository.GetProductsBySearch(tempCatalogInfo, search, searchModel);
            }

            ret = ApplySpecialFilters(catalogInfo, profile, specialFilters, searchModel, ret);

            AddPricingInfo(ret, catalogInfo, searchModel);
            GetAdditionalProductInfo(profile, ret, catalogInfo);
            ret.CatalogCounts = catalogCounts;

            foreach (var product in ret.Products)
            {
                product.IsSpecialtyCatalog = _catalogLogic.IsSpecialtyCatalog(searchModel.CatalogType);
            }

            return ret;
        }

        private ProductsReturn GetAllProductsBySearchShallow(string search, SearchInputModel searchModel,
            UserSelectedContext tempCatalogInfo)
        {
            searchModel.Size = 1; // set size to 1 to make results small; we want total
            ProductsReturn ret = _catalogRepository.GetProductNumbersBySearch(tempCatalogInfo, search, searchModel);

            searchModel.Size = ret.TotalCount; // set size to total to get all
            ret = _catalogRepository.GetProductNumbersBySearch(tempCatalogInfo, search, searchModel);
            return ret;
        }

        #endregion

        #region by house products
        public ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo, 
                                                       string brandControlLabel, 
                                                       SearchInputModel searchModel, 
                                                       UserProfile profile)
        {
            ProductsReturn returnValue;

            List<string> specialFilters = _catalogRepository.SeekSpecialFilters(searchModel.Facets);

            // special handling for price sorting
            if (searchModel.SField == "caseprice")
                returnValue = _catalogRepository.GetHouseProductsByBranch(catalogInfo, brandControlLabel, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = Configuration.MaxSortByPriceItemCount });
            else if (specialFilters.Count>0)
            {
                returnValue = GetAllHouseBrandProductsShallow(catalogInfo, brandControlLabel, searchModel);
            }
            else
            {
                returnValue = _catalogRepository.GetHouseProductsByBranch(catalogInfo, brandControlLabel, searchModel);
            }

            returnValue = ApplySpecialFilters(catalogInfo, profile, specialFilters, searchModel, returnValue);

            AddPricingInfo(returnValue, catalogInfo, searchModel);
            GetAdditionalProductInfo(profile, returnValue, catalogInfo);

            return returnValue;
        }

        private ProductsReturn GetAllHouseBrandProductsShallow(UserSelectedContext catalogInfo, string brandControlLabel,
            SearchInputModel searchModel)
        {
            searchModel.Size = 1; // set size to 1; we want total
            ProductsReturn returnValue = _catalogRepository.GetHouseProductNumbersByBranch(catalogInfo, brandControlLabel,
                searchModel);

            searchModel.Size = returnValue.TotalCount; // set size to total
            returnValue = _catalogRepository.GetHouseProductNumbersByBranch(catalogInfo, brandControlLabel, searchModel);
            return returnValue;
        }

        #endregion

        #region common
        private ProductsReturn ApplySpecialFilters(UserSelectedContext catalogInfo, UserProfile profile, List<string> specialFilters, SearchInputModel searchModel,
            ProductsReturn returnProducts)
        {
            if (specialFilters != null && specialFilters.Count > 0)
            {
                if (specialFilters.Contains(Constants.SPECIALFILTER_DEVIATEDPRICES))
                {
                    FilterDeviatedPriceProducts(returnProducts, catalogInfo);
                }

                if (specialFilters.Contains(Constants.SPECIALFILTER_PREVIOUSORDERED))
                {
                    FilterPreviouslyOrderedProducts(catalogInfo, profile, returnProducts);
                }

                ProductsReturn filtered = _catalogRepository.GetProductsByIds(catalogInfo.BranchId,
                    returnProducts.Products.Select(p => p.ItemNumber).ToList());

                // add facet for specialfilters to return and set count to number of products
                filtered.Facets = returnProducts.Facets;
                _catalogRepository.RecalculateFacets(filtered, specialFilters);
                returnProducts = filtered;
            }
            return returnProducts;
        }

        /// <summary>
        /// Filter the given list of products for just those with deviated prices
        /// </summary>
        /// <param name="prods"></param>
        /// <param name="context"></param>
        /// <param name="searchModel"></param>
        private void FilterDeviatedPriceProducts(ProductsReturn prods, UserSelectedContext context)
        {
            if (context == null || String.IsNullOrEmpty(context.CustomerId))
                return;

            PriceReturn pricingInfo = GetPricingInfoForProducts(prods, context);

            foreach (Price p in pricingInfo.Prices)
            {
                Product prod = prods.Products.Find(x => x.ItemNumber == p.ItemNumber);

                prod.DeviatedCost = p.DeviatedCost ? "Y" : "N";
            }

            prods.Products = prods.Products.Where(p => p.DeviatedCost == "Y").ToList();
            prods.Count = prods.Products.Count();
            prods.TotalCount = prods.Products.Count();
        }

        private void FilterPreviouslyOrderedProducts(UserSelectedContext catalogInfo, UserProfile profile, ProductsReturn ret)
        {
            // filter out just those that are recently ordered
            GetPreviouslyOrderedProductInfo(profile, ret, catalogInfo);
            ret.Products = ret.Products
                              .Where(p => p.InHistory)
                              .ToList();
        }

        private PriceReturn GetPricingInfoForProducts(ProductsReturn prods, UserSelectedContext context)
        {
            PriceReturn pricingInfo = null;
            if (prods.Products.Count > 0 && _catalogLogic.IsSpecialtyCatalog(null, prods.Products[0].CatalogId))
            {
                string source = _catalogLogic.GetCatalogTypeFromCatalogId(prods.Products[0].CatalogId);
                pricingInfo = _priceLogic.GetNonBekItemPrices("fdf", context.CustomerId, source, DateTime.Now.AddDays(1), prods.Products);
            }
            else
            {
                pricingInfo = _priceLogic.GetPrices(context.BranchId, context.CustomerId, DateTime.Now.AddDays(1), prods.Products);
            }

            return pricingInfo;
        }

        public void AddPricingInfo(ProductsReturn prods, UserSelectedContext context, SearchInputModel searchModel)
        {
            if (context == null || String.IsNullOrEmpty(context.CustomerId))
                return;

            PriceReturn pricingInfo = GetPricingInfoForProducts(prods, context);

            AddPricingInfoToProducts(prods, pricingInfo);

            AddSortForPricingWhenApplicable(prods, searchModel);
        }

        private void AddPricingInfoToProducts(ProductsReturn prods, PriceReturn pricingInfo)
        {
            foreach (Price p in pricingInfo.Prices)
            {
                Product prod = prods.Products.Find(x => x.ItemNumber == p.ItemNumber);

                prod.CasePrice = p.CasePrice.ToString();
                prod.CasePriceNumeric = p.CasePrice;
                prod.PackagePrice = p.PackagePrice.ToString();
                prod.PackagePriceNumeric = p.PackagePrice;
                prod.DeviatedCost = p.DeviatedCost ? "Y" : "N";
                //}
            }
        }

        private void AddSortForPricingWhenApplicable(ProductsReturn prods, SearchInputModel searchModel)
        {
            if ((searchModel.SField == "caseprice"
                || searchModel.SField == "unitprice")) // sort pricing info first
            {
                if (searchModel.SDir == "asc")
                    SortAscendingByPrices(prods, searchModel);
                else
                    SortDescendingByPrices(prods, searchModel);
            }
        }

        private void SortDescendingByPrices(ProductsReturn prods, SearchInputModel searchModel)
        {
            if (searchModel.SField == "caseprice")
                prods.Products.Sort((x, y) => y.CasePriceNumeric.CompareTo(x.CasePriceNumeric));
            else
                prods.Products.Sort((x, y) => y.UnitCost.CompareTo(x.UnitCost));
        }

        private void SortAscendingByPrices(ProductsReturn prods, SearchInputModel searchModel)
        {
            if (searchModel.SField == "caseprice")
                prods.Products.Sort((x, y) => x.CasePriceNumeric.CompareTo(y.CasePriceNumeric));
            else
                prods.Products.Sort((x, y) => x.UnitCost.CompareTo(y.UnitCost));
        }

        private void GetAdditionalProductInfo(UserProfile profile, ProductsReturn ret, UserSelectedContext catalogInfo)
        {
            if (profile != null)
            {
                ListModel notes = _notesLogic.GetList(catalogInfo);
                List<string> favorites = _favoritesLogic.GetFavoritedItemNumbers(profile, catalogInfo);
                var history = _historyLogic.ItemsInHistoryList(catalogInfo, ret.Products.Select(p => p.ItemNumber).ToList());

                ret.Products.ForEach(delegate (Product prod) {
                    prod.Favorite = favorites.Contains(prod.ItemNumber);
                    prod.Notes = notes.Items.Where(n => n.ItemNumber.Equals(prod.ItemNumber))
                                      .Select(i => i.Notes)
                                      .FirstOrDefault();
                    prod.InHistory = history.Where(h => h.ItemNumber.Equals(prod.ItemNumber))
                                            .FirstOrDefault()
                                            .InHistory;
                    _catalogLogic.AddProductImageInfo(prod);
                });
            }
        }

        private void GetPreviouslyOrderedProductInfo(UserProfile profile, ProductsReturn ret, UserSelectedContext catalogInfo)
        {
            if (profile != null)
            {
                var history = _historyLogic.ItemsInHistoryList(catalogInfo, ret.Products.Select(p => p.ItemNumber).ToList());

                ret.Products.ForEach(delegate (Product prod) {
                    prod.InHistory = history.Where(h => h.ItemNumber.Equals(prod.ItemNumber))
                                            .FirstOrDefault()
                                            .InHistory;
                });
            }
        }
        #endregion
        #endregion
    }
}
