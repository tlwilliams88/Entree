using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Extensions;

namespace KeithLink.Svc.Impl.Logic
{
    public class SiteCatalogLogicImpl : ICatalogLogic
    {
        #region attributes
        private ICatalogRepository _catalogRepository;
        private IPriceLogic _priceLogic;
        private IProductImageRepository _imgRepository;
        private IListLogic _listLogic;
		private IDivisionRepository _divisionRepository;
        private ICategoryImageRepository _categoryImageRepository;
        private ICatalogCacheRepository _catalogCacheRepository;
        #endregion

        public SiteCatalogLogicImpl(ICatalogRepository catalogRepository, IPriceLogic priceLogic, IProductImageRepository imgRepository, IListLogic listLogic, IDivisionRepository divisionRepository, ICategoryImageRepository categoryImageRepository, ICatalogCacheRepository catalogCacheRepository)
        {
            _catalogRepository = catalogRepository;
            _priceLogic = priceLogic;
            _imgRepository = imgRepository;
            _listLogic = listLogic;
			_divisionRepository = divisionRepository;
            _categoryImageRepository = categoryImageRepository;
            _catalogCacheRepository = catalogCacheRepository;
        }

        public CategoriesReturn GetCategories(int from, int size)
        {
            CategoriesReturn categoriesReturn = _catalogCacheRepository.GetItem<CategoriesReturn>(GetCategoriesCacheKey(from, size));
            if (categoriesReturn == null)
            {
                categoriesReturn = _catalogRepository.GetCategories(from, size);
                AddCategoryImages(categoriesReturn);
                AddCategorySearchName(categoriesReturn);
                _catalogCacheRepository.AddItem<CategoriesReturn>(GetCategoriesCacheKey(from, size), categoriesReturn);
            }
            return categoriesReturn;
        }

        private static string GetCategoriesCacheKey(int from, int size)
        {
            return "CategoriesReturn_" + from + "_" + size;
        }

        public Product GetProductById(string branch, string id, UserProfile profile)
        {
            Product ret = _catalogRepository.GetProductById(branch, id);
            AddFavoriteProductInfo(branch, profile, ret);
            AddProductImageInfo(ret);
            return ret;
        }

        private void AddCategoryImages(CategoriesReturn returnValue)
        {
            foreach (Category c in returnValue.Categories)
            {
                c.CategoryImage = _categoryImageRepository.GetImageByCategory(c.Id).CategoryImage;
            }
        }

        private void AddCategorySearchName(CategoriesReturn returnValue)
        {
            foreach (Category c in returnValue.Categories)
            {
                c.SearchName = GetCategorySearchName(c.Name);
                foreach (SubCategory sc in c.SubCategories)
                    sc.SearchName = GetCategorySearchName(sc.Name);
            }
        }

        private string GetCategorySearchName(string categoryName)
        {
            // remove ',' and '.', replace '&' with 'and', replace white space and / with _, lowercase
            if (!String.IsNullOrEmpty(categoryName))
                return categoryName.Replace("&", "and").Replace(",", "").Replace(" ", "_").Replace("/","_").Replace(".","").ToLower();
            return categoryName;
        }

        private void AddProductImageInfo(Product ret)
        {
            ret.ProductImages = _imgRepository.GetImageList(ret.ItemNumber).ProductImages;
        }

        public ProductsReturn GetProductsByIds(string branch, List<string> ids, UserProfile profile)
        {
            ProductsReturn ret = _catalogRepository.GetProductsByIds(branch, ids);
            return ret;
        }

		public ProductsReturn GetProductsByCategory(CatalogInfo catalogInfo, string category, SearchInputModel searchModel, UserProfile profile)
        {
            ProductsReturn ret;
            string categoryName = category;

            // enable category search on either category id or search name
            Category catFromSearchName = this.GetCategories(0, 2000).Categories.Where(x => x.SearchName == category).FirstOrDefault();
            if (catFromSearchName != null)
                categoryName = catFromSearchName.Name;

            // special handling for price sorting
            if (searchModel.SField == "caseprice")
                ret = _catalogRepository.GetProductsByCategory(catalogInfo, categoryName, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = Configuration.MaxSortByPriceItemCount });
            else
                ret = _catalogRepository.GetProductsByCategory(catalogInfo, categoryName, searchModel);

            AddPricingInfo(ret, profile, searchModel);
            AddFavoriteProductInfoAndNotes(catalogInfo.BranchId, profile, ret);
            return ret;
        }

		public ProductsReturn GetHouseProductsByBranch(CatalogInfo catalogInfo, string brandControlLabel, SearchInputModel searchModel, UserProfile profile)
        {
            ProductsReturn returnValue;

            // special handling for price sorting
            if (searchModel.SField == "caseprice")
                returnValue = _catalogRepository.GetHouseProductsByBranch(catalogInfo, brandControlLabel, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = Configuration.MaxSortByPriceItemCount });
            else
                returnValue = _catalogRepository.GetHouseProductsByBranch(catalogInfo, brandControlLabel, searchModel);

            AddPricingInfo(returnValue, profile, searchModel);
            AddFavoriteProductInfoAndNotes(catalogInfo.BranchId, profile, returnValue);

            return returnValue;
        }

        public ProductsReturn GetProductsBySearch(CatalogInfo catalogInfo, string search, SearchInputModel searchModel, UserProfile profile)
        {
            ProductsReturn ret;

            // special handling for price sorting
            if (searchModel.SField == "caseprice")
				ret = _catalogRepository.GetProductsBySearch(catalogInfo, search, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = Configuration.MaxSortByPriceItemCount });
            else
				ret = _catalogRepository.GetProductsBySearch(catalogInfo, search, searchModel);
                
            AddPricingInfo(ret, profile, searchModel);
			AddFavoriteProductInfoAndNotes(catalogInfo.BranchId, profile, ret);
            return ret;
        }

        private void AddPricingInfo(ProductsReturn prods, UserProfile profile, SearchInputModel searchModel)
        {
            if (profile == null)
                return;

            PriceReturn pricingInfo = _priceLogic.GetPrices(profile.BranchId, profile.CustomerNumber, DateTime.Now.AddDays(1), prods.Products);

            foreach (Price p in pricingInfo.Prices)
            {
                Product prod = prods.Products.Find(x => x.ItemNumber == p.ItemNumber);
                prod.CasePrice = String.Format("{0:C}", p.CasePrice);
                prod.CasePriceNumeric = p.CasePrice;
                prod.PackagePrice = String.Format("{0:C}", p.PackagePrice);
            }

            if (searchModel.SField == "caseprice" && prods.TotalCount <= Configuration.MaxSortByPriceItemCount) // sort pricing info first
            {
                if (searchModel.SDir == "asc")
                    prods.Products.Sort((x, y) => x.CasePriceNumeric.CompareTo(y.CasePriceNumeric));
                else
                    prods.Products.Sort((x, y) => y.CasePriceNumeric.CompareTo(x.CasePriceNumeric));
            }
        }

        private void AddFavoriteProductInfo(string branch, UserProfile profile, Product ret)
        {
            if (profile != null)
                _listLogic.MarkFavoriteProductsAndNotes(profile.UserId, branch, new ProductsReturn() { Products = new List<Product>() { ret } });
        }

        private void AddFavoriteProductInfoAndNotes(string branch, UserProfile profile, ProductsReturn ret)
        {
            if (profile != null)
                _listLogic.MarkFavoriteProductsAndNotes(profile.UserId, branch, ret);
        }


		public List<Division> GetDivisions()
		{
			var catalogs = _divisionRepository.GetDivisions();

			return catalogs.Select(c => c.ToDivision()).ToList();
		}
	}
}
