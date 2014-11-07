using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Extensions;

namespace KeithLink.Svc.Impl.Logic.SiteCatalog
{
    public class SiteCatalogLogicImpl : ICatalogLogic
    {
        #region attributes
        private ICatalogRepository _catalogRepository;
        private IPriceLogic _priceLogic;
        private IProductImageRepository _imgRepository;
		private IDivisionRepository _divisionRepository;
        private ICategoryImageRepository _categoryImageRepository;
        private ICatalogCacheRepository _catalogCacheRepository;
		private IListServiceRepository _listServiceRepository;
		private IDivisionLogic _divisionLogic;
        private IOrderHistoryHeaderRepsitory _orderHistoryRepository;
        #endregion

        public SiteCatalogLogicImpl(ICatalogRepository catalogRepository, IPriceLogic priceLogic, IProductImageRepository imgRepository, IListServiceRepository listServiceRepository, IDivisionRepository divisionRepository, ICategoryImageRepository categoryImageRepository, ICatalogCacheRepository catalogCacheRepository, IDivisionLogic divisionLogic, IOrderHistoryHeaderRepsitory orderHistoryRepository)
        {
            _catalogRepository = catalogRepository;
            _priceLogic = priceLogic;
            _imgRepository = imgRepository;
			_listServiceRepository = listServiceRepository;
			_divisionRepository = divisionRepository;
            _categoryImageRepository = categoryImageRepository;
            _catalogCacheRepository = catalogCacheRepository;
			_divisionLogic = divisionLogic;
            _orderHistoryRepository = orderHistoryRepository;
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
            return String.Format("CategoriesReturn_{0}_{1}", from, size);
        }

        public Product GetProductById(UserSelectedContext catalogInfo, string id, UserProfile profile)
        {
            Product ret = _catalogRepository.GetProductById(catalogInfo.BranchId, id);
			AddFavoriteProductInfo(profile, ret, catalogInfo);
            AddProductImageInfo(ret);
            AddItemHistoryToProduct( ret, catalogInfo );

			PriceReturn pricingInfo = _priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), new List<Product>() { ret });

			if (pricingInfo != null && pricingInfo.Prices.Where(p => p.ItemNumber.Equals(ret.ItemNumber)).Any())
			{
				var price = pricingInfo.Prices.Where(p => p.ItemNumber.Equals(ret.ItemNumber)).First();
				ret.CasePrice = String.Format("{0:C}", price.CasePrice);
				ret.CasePriceNumeric = price.CasePrice;
				ret.PackagePrice = String.Format("{0:C}", price.PackagePrice);
                ret.DeviatedCost = price.DeviatedCost ? "Y" : "N";
			}
			
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

        private void AddItemHistoryToProduct( Product returnValue, UserSelectedContext catalogInfo ) {
            return; // TODO: Refactor to use Order Svc
            IEnumerable<OrderHistoryHeader> history = _orderHistoryRepository.GetLastFiveOrdersByItem( catalogInfo.BranchId, catalogInfo.CustomerId, returnValue.ItemNumber );

            foreach (OrderHistoryHeader h in history) {
                foreach (OrderHistoryDetail d in h.OrderDetails.Where(x => x.ItemNumber.Equals(returnValue.ItemNumber))) {
                    returnValue.OrderHistory.Add( h.DeliveryDate.Value.ToShortDateString(), d.ShippedQuantity );
                }
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

		public ProductsReturn GetProductsByCategory(UserSelectedContext catalogInfo, string category, SearchInputModel searchModel, UserProfile profile)
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

            AddPricingInfo(ret, profile, catalogInfo, searchModel);
            AddFavoriteProductInfoAndNotes(catalogInfo.BranchId, profile, ret, catalogInfo);
            return ret;
        }

		public ProductsReturn GetHouseProductsByBranch(UserSelectedContext catalogInfo, string brandControlLabel, SearchInputModel searchModel, UserProfile profile)
        {
            ProductsReturn returnValue;

            // special handling for price sorting
            if (searchModel.SField == "caseprice")
                returnValue = _catalogRepository.GetHouseProductsByBranch(catalogInfo, brandControlLabel, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = Configuration.MaxSortByPriceItemCount });
            else
                returnValue = _catalogRepository.GetHouseProductsByBranch(catalogInfo, brandControlLabel, searchModel);

            AddPricingInfo(returnValue, profile, catalogInfo, searchModel);
            AddFavoriteProductInfoAndNotes(catalogInfo.BranchId, profile, returnValue, catalogInfo);

            return returnValue;
        }

        public ProductsReturn GetProductsBySearch(UserSelectedContext catalogInfo, string search, SearchInputModel searchModel, UserProfile profile)
        {
            ProductsReturn ret;

            // special handling for price sorting
            if (searchModel.SField == "caseprice")
				ret = _catalogRepository.GetProductsBySearch(catalogInfo, search, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = Configuration.MaxSortByPriceItemCount });
            else
				ret = _catalogRepository.GetProductsBySearch(catalogInfo, search, searchModel);
                
            AddPricingInfo(ret, profile, catalogInfo, searchModel);
			AddFavoriteProductInfoAndNotes(catalogInfo.BranchId, profile, ret, catalogInfo);
            return ret;
        }

        private void AddPricingInfo(ProductsReturn prods, UserProfile profile, UserSelectedContext context, SearchInputModel searchModel)
        {
            if (profile == null || context == null || String.IsNullOrEmpty(context.CustomerId))
                return;

            PriceReturn pricingInfo = _priceLogic.GetPrices(context.BranchId, context.CustomerId, DateTime.Now.AddDays(1), prods.Products);

            foreach (Price p in pricingInfo.Prices)
            {
                Product prod = prods.Products.Find(x => x.ItemNumber == p.ItemNumber);
                prod.CasePrice = String.Format("{0:C}", p.CasePrice);
                prod.CasePriceNumeric = p.CasePrice;
                prod.PackagePrice = String.Format("{0:C}", p.PackagePrice);
                prod.DeviatedCost = p.DeviatedCost ? "Y" : "N";
            }

            if (searchModel.SField == "caseprice" && prods.TotalCount <= Configuration.MaxSortByPriceItemCount) // sort pricing info first
            {
                if (searchModel.SDir == "asc")
                    prods.Products.Sort((x, y) => x.CasePriceNumeric.CompareTo(y.CasePriceNumeric));
                else
                    prods.Products.Sort((x, y) => y.CasePriceNumeric.CompareTo(x.CasePriceNumeric));
            }
        }

		private void AddFavoriteProductInfo(UserProfile profile, Product ret, UserSelectedContext catalogInfo)
        {
			if (profile != null)
			{
				var list = _listServiceRepository.ReadFavorites(profile, catalogInfo);
				var notes = _listServiceRepository.ReadNotes(profile, catalogInfo);

				ret.Favorite = list.Contains(ret.ItemNumber);
				ret.Notes = notes.Where(n => n.ItemNumber.Equals(ret.ItemNumber)).Select(i => i.Notes).FirstOrDefault();
			}
        }

		private void AddFavoriteProductInfoAndNotes(string branch, UserProfile profile, ProductsReturn ret, UserSelectedContext catalogInfo)
        {
			if (profile != null)
			{
				var favorites = _listServiceRepository.ReadFavorites(profile, catalogInfo);
				var notes = _listServiceRepository.ReadNotes(profile, catalogInfo);

				ret.Products.ForEach(delegate (Product prod) 
				{
					prod.Favorite = favorites.Contains(prod.ItemNumber);
					prod.Notes = notes.Where(n => n.ItemNumber.Equals(prod.ItemNumber)).Select(i => i.Notes).FirstOrDefault();
				});
			}
        }


		public List<Division> GetDivisions()
		{
			return _divisionLogic.GetDivisions();
		}
	}
}
