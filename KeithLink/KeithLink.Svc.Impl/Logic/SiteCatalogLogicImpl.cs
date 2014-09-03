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

namespace KeithLink.Svc.Impl.Logic
{
    public class SiteCatalogLogicImpl : ICatalogLogic
    {
        #region attributes
        private ICatalogRepository _catalogRepository;
        private IPriceLogic _priceLogic;
        private IProductImageRepository _imgRepository;
        private IListLogic _listLogic;
        #endregion

        public SiteCatalogLogicImpl(ICatalogRepository catalogRepository, IPriceLogic priceLogic, IProductImageRepository imgRepository, IListLogic listLogic)
        {
            _catalogRepository = catalogRepository;
            _priceLogic = priceLogic;
            _imgRepository = imgRepository;
            _listLogic = listLogic;
        }

        public CategoriesReturn GetCategories(int from, int size)
        {
            return _catalogRepository.GetCategories(from, size);
        }

        public Product GetProductById(string branch, string id, UserProfile profile)
        {
            Product ret = _catalogRepository.GetProductById(branch, id);
            AddFavoriteProductInfo(branch, profile, ret);
            AddProductImageInfo(ret);
            return ret;
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

        public ProductsReturn GetProductsByCategory(string branch, string category, SearchInputModel searchModel, UserProfile profile)
        {
            ProductsReturn ret;
            
            // special handling for price sorting
            if (searchModel.SField == "caseprice")
                ret = _catalogRepository.GetProductsByCategory(branch, category, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = Configuration.MaxSortByPriceItemCount });
            else
                ret = _catalogRepository.GetProductsByCategory(branch, category, searchModel);

            AddPricingInfo(ret, profile, searchModel);
            AddFavoriteProductInfo(branch, profile, ret);
            return ret;
        }

        public ProductsReturn GetProductsBySearch(string branch, string search, SearchInputModel searchModel, UserProfile profile)
        {
            ProductsReturn ret;

            // special handling for price sorting
            if (searchModel.SField == "caseprice")
                ret = _catalogRepository.GetProductsBySearch(branch, search, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = Configuration.MaxSortByPriceItemCount });
            else
                ret = _catalogRepository.GetProductsBySearch(branch, search, searchModel);
                
            AddPricingInfo(ret, profile, searchModel);
            AddFavoriteProductInfo(branch, profile, ret);
            return ret;
        }

        private void AddPricingInfo(ProductsReturn prods, UserProfile profile, SearchInputModel searchModel)
        {
            if (profile == null)
                return;

            PriceReturn pricingInfo = _priceLogic.GetPrices(profile.BranchId, profile.CustomerId, DateTime.Now.AddDays(1), prods.Products);

            foreach (Price p in pricingInfo.Prices)
            {
                Product prod = prods.Products.Find(x => x.ItemNumber == p.ItemNumber);
                prod.CasePrice = String.Format("{0:C}", p.CasePrice);
                prod.PackagePrice = String.Format("{0:C}", p.PackagePrice);
            }

            if (searchModel.SField == "caseprice" && prods.TotalCount <= Configuration.MaxSortByPriceItemCount) // sort pricing info first
            {
                if (searchModel.SDir == "asc")
                    prods.Products.Sort((x, y) => x.CasePrice.CompareTo(y.CasePrice));
                else
                    prods.Products.Sort((x, y) => y.CasePrice.CompareTo(x.CasePrice));
            }
        }

        private void AddFavoriteProductInfo(string branch, UserProfile profile, Product ret)
        {
            if (profile != null)
                _listLogic.MarkFavoriteProducts(profile.UserId, branch, new ProductsReturn() { Products = new List<Product>() { ret } });
        }

        private void AddFavoriteProductInfo(string branch, UserProfile profile, ProductsReturn ret)
        {
            if (profile != null)
                _listLogic.MarkFavoriteProducts(profile.UserId, branch, ret);
        }
    }
}
