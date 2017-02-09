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
        private readonly IFavoriteLogic _favoriteLogic;
        private readonly IHistoryLogic _historyLogic;
        private readonly IItemHistoryRepository _itemHistoryRepo;
        private readonly INoteLogic _noteLogic;

        protected string CACHE_GROUPNAME { get { return "Catalog"; } }
        protected string CACHE_NAME { get { return "Catalog"; } }
        protected string CACHE_PREFIX { get { return "Default"; } }
        #endregion

        #region constructor
        public SiteCatalogServiceImpl(ICacheRepository catalogCacheRepository, ICatalogLogic catalogLogic
                                      , ICatalogRepository catalogRepository, IPriceLogic priceLogic
                                      , IListRepository listRepo, IFavoriteLogic favoriteLogic
                                      , IHistoryLogic historyLogic, INoteLogic noteLogic
                                      , IItemHistoryRepository itemHistoryRepo)
        {
            _catalogCacheRepository = catalogCacheRepository;
            _catalogLogic = catalogLogic;
            _catalogRepository = catalogRepository;
            _priceLogic = priceLogic;
            _listRepo = listRepo;
            _favoriteLogic = favoriteLogic;
            _historyLogic = historyLogic;
            _itemHistoryRepo = itemHistoryRepo;
            _noteLogic = noteLogic;
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
            else if (specialFilters.Contains(Constants.SPECIALFILTER_DEVIATEDPRICES))
            {
                ret = GetDeviatedPriceProductsByCategory(catalogInfo, searchModel, categoryName, newCatalog);
            }
            else if (specialFilters.Contains(Constants.SPECIALFILTER_RECENTLYORDERED))
            {
                ret = GetRecentlyOrderedProductsByCategory(catalogInfo, searchModel, categoryName, newCatalog, profile);
            }
            else
            {
                ret = _catalogRepository.GetProductsByCategory(newCatalog, categoryName, searchModel);
            }

            AddPricingInfo(ret, catalogInfo, searchModel);

            GetAdditionalProductInfo(profile, ret, catalogInfo);

            return ret;
        }

        private ProductsReturn GetRecentlyOrderedProductsByCategory(UserSelectedContext catalogInfo,
                                                                    SearchInputModel searchModel,
                                                                    string categoryName,
                                                                    UserSelectedContext newCatalog,
                                                                    UserProfile profile)
        {
            // get just the itemnumber and catalogid of the products matching the query
            ProductsReturn ret = _catalogRepository.GetProductNumbersByCategory(newCatalog, categoryName, searchModel);

            FilterPreviouslyOrderedProducts(catalogInfo, profile, ret);

            // now go back and fill out the rest of the product information on those with deviated prices
            ret = _catalogRepository.GetProductsByIds(newCatalog.BranchId, ret.Products.Select(p => p.ItemNumber).ToList());
            GetPreviouslyOrderedProductInfo(profile, ret, catalogInfo);

            // add facet for specialfilters to return and set count to number of products
            ret.Facets = new System.Dynamic.ExpandoObject();
            _catalogRepository.AddSpecialFiltersToFacets(ret.Facets, ret.Products.Count().ToString());

            return ret;
        }

        private ProductsReturn GetDeviatedPriceProductsByCategory(UserSelectedContext catalogInfo,
                                                                  SearchInputModel searchModel,
                                                                  string categoryName,
                                                                  UserSelectedContext newCatalog)
        {
            // get just the itemnumber and catalogid of the products matching the query
            ProductsReturn ret = _catalogRepository.GetProductNumbersByCategory(newCatalog, categoryName, searchModel);

            // filter out just those with deviated prices
            FilterDeviatedPriceProducts(ret, catalogInfo);

            // now go back and fill out the rest of the product information on those with deviated prices
            ret = _catalogRepository.GetProductsByIds(newCatalog.BranchId, ret.Products.Select(p => p.ItemNumber).ToList());

            // add facet for specialfilters to return and set count to number of products
            ret.Facets = new System.Dynamic.ExpandoObject();
            _catalogRepository.AddSpecialFiltersToFacets(ret.Facets, ret.Products.Count().ToString());

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
            else if (specialFilters.Contains("deviatedprices"))
            {
                ret = GetDeviatedPriceProductsBySearch(catalogInfo, search, searchModel, tempCatalogInfo);
            }
            else if (specialFilters.Contains("recentlyordered"))
            {
                ret = GetRecentlyOrderedProductsBySearch(catalogInfo, search, searchModel, tempCatalogInfo, profile);
            }
            else
            {
                ret = _catalogRepository.GetProductsBySearch(tempCatalogInfo, search, searchModel);
            }

            AddPricingInfo(ret, catalogInfo, searchModel);
            GetAdditionalProductInfo(profile, ret, catalogInfo);
            ret.CatalogCounts = catalogCounts;

            foreach (var product in ret.Products)
            {
                product.IsSpecialtyCatalog = _catalogLogic.IsSpecialtyCatalog(searchModel.CatalogType);
            }

            return ret;
        }

        private ProductsReturn GetDeviatedPriceProductsBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel, UserSelectedContext tempCatalogInfo)
        {
            ProductsReturn ret = _catalogRepository.GetProductNumbersBySearch(tempCatalogInfo, search, searchModel);
            // filter out just those with deviated prices
            FilterDeviatedPriceProducts(ret, catalogInfo);
            // now go back and fill out the rest of the product information on those with deviated prices
            ret = _catalogRepository.GetProductsByIds(catalogInfo.BranchId, ret.Products.Select(p => p.ItemNumber).ToList());
            // add facet for specialfilters to return and set count to number of products
            ret.Facets = new System.Dynamic.ExpandoObject();
            _catalogRepository.AddSpecialFiltersToFacets(ret.Facets, ret.Products.Count().ToString());
            return ret;
        }

        private ProductsReturn GetRecentlyOrderedProductsBySearch(UserSelectedContext catalogInfo, 
                                                                  string search, 
                                                                  SearchInputModel searchModel, 
                                                                  UserSelectedContext tempCatalogInfo,
                                                                  UserProfile profile)
        {
            ProductsReturn ret = _catalogRepository.GetProductNumbersBySearch(tempCatalogInfo, search, searchModel);

            FilterPreviouslyOrderedProducts(catalogInfo, profile, ret);

            // now go back and fill out the rest of the product information on those that are recently ordered
            ret = _catalogRepository.GetProductsByIds(catalogInfo.BranchId, ret.Products.Select(p => p.ItemNumber).ToList());

            // add facet for specialfilters to return and set count to number of products
            ret.Facets = new System.Dynamic.ExpandoObject();
            _catalogRepository.AddSpecialFiltersToFacets(ret.Facets, ret.Products.Count().ToString());
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
            else if (specialFilters.Contains("deviatedprices"))
            {
                returnValue = GetDeviatedPriceHouseProductsByBranch(catalogInfo, brandControlLabel, searchModel);
            }
            else
            {
                returnValue = _catalogRepository.GetHouseProductsByBranch(catalogInfo, brandControlLabel, searchModel);
            }

            AddPricingInfo(returnValue, catalogInfo, searchModel);
            GetAdditionalProductInfo(profile, returnValue, catalogInfo);

            return returnValue;
        }

        private ProductsReturn GetDeviatedPriceHouseProductsByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel)
        {
            ProductsReturn returnValue = _catalogRepository.GetHouseProductNumbersByBranch(catalogInfo, brandControlLabel, searchModel);
            // filter out just those with deviated prices
            FilterDeviatedPriceProducts(returnValue, catalogInfo);
            // now go back and fill out the rest of the product information on those with deviated prices
            returnValue = _catalogRepository.GetProductsByIds(catalogInfo.BranchId, returnValue.Products.Select(p => p.ItemNumber).ToList());
            // add facet for specialfilters to return and set count to number of products
            returnValue.Facets = new System.Dynamic.ExpandoObject();
            _catalogRepository.AddSpecialFiltersToFacets(returnValue.Facets, returnValue.Products.Count().ToString());
            return returnValue;
        }

        private ProductsReturn GetRecentlyOrderedHouseProductsByBranch(UserSelectedContext catalogInfo, 
                                                                       string brandControlLabel, 
                                                                       SearchInputModel searchModel,
                                                                       UserProfile profile)
        {
            ProductsReturn returnValue = _catalogRepository.GetHouseProductNumbersByBranch(catalogInfo, brandControlLabel, searchModel);

            FilterPreviouslyOrderedProducts(catalogInfo, profile, returnValue);

            // now go back and fill out the rest of the product information on those with deviated prices
            returnValue = _catalogRepository.GetProductsByIds(catalogInfo.BranchId, returnValue.Products.Select(p => p.ItemNumber).ToList());

            // add facet for specialfilters to return and set count to number of products
            returnValue.Facets = new System.Dynamic.ExpandoObject();
            _catalogRepository.AddSpecialFiltersToFacets(returnValue.Facets, returnValue.Products.Count().ToString());
            return returnValue;
        }
        #endregion

        #region common
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
                var favorites = _favoriteLogic.GetFavoritedItemNumbers(profile, catalogInfo);
                var notes = _noteLogic.GetNotes(profile, catalogInfo);
                var history = _historyLogic.ItemsInHistoryList(catalogInfo, ret.Products.Select(p => p.ItemNumber).ToList());

                ret.Products.ForEach(delegate (Product prod) {
                    prod.Favorite = favorites.Contains(prod.ItemNumber);
                    prod.Notes = notes.Where(n => n.ItemNumber.Equals(prod.ItemNumber))
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
                    _catalogLogic.AddProductImageInfo(prod);
                });
            }
        }
        #endregion
        #endregion
    }
}
