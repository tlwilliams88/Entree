using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Impl.Logic
{
    public class SiteCatalogLogicImpl : ICatalogLogic
    {
        #region attributes
        private ICatalogRepository _catalogRepository;
        private IPriceLogic _priceLogic;
        #endregion

        public SiteCatalogLogicImpl(ICatalogRepository catalogRepository, IPriceLogic priceLogic)
        {
            _catalogRepository = catalogRepository;
            _priceLogic = priceLogic;
        }

        public CategoriesReturn GetCategories(int from, int size)
        {
            return _catalogRepository.GetCategories(from, size);
        }

        public Product GetProductById(string branch, string id, UserProfile profile)
        {
            Product ret = _catalogRepository.GetProductById(branch, id);
            return ret;
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
            if (searchModel.SField == "price")
                ret = _catalogRepository.GetProductsByCategory(branch, category, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = 200 });
            else
                ret = _catalogRepository.GetProductsByCategory(branch, category, searchModel);

            AddPricingInfo(ret, profile, searchModel);
            return ret;
        }

        public ProductsReturn GetProductsBySearch(string branch, string search, SearchInputModel searchModel, UserProfile profile)
        {
            ProductsReturn ret;

            // special handling for price sorting
            if (searchModel.SField == "price")
                ret = _catalogRepository.GetProductsBySearch(branch, search, new SearchInputModel() { Facets = searchModel.Facets, From = searchModel.From, Size = 200 });
            else
                ret = _catalogRepository.GetProductsBySearch(branch, search, searchModel);
                
            AddPricingInfo(ret, profile, searchModel);
            return ret;
        }

        private void AddPricingInfo(ProductsReturn prods, UserProfile profile, SearchInputModel searchModel)
        {
            if (profile == null)
                return;

            PriceReturn pricingInfo = _priceLogic.GetPrices(profile.BranchId, profile.CustomerId, DateTime.Now.AddDays(1), prods.Products);

            if (searchModel.SField == "price" && prods.TotalCount <= 200) // sort pricing info first
            {
                if (searchModel.SDir == "asc")
                    pricingInfo.Prices.Sort((x, y) => x.PackagePrice.CompareTo(y.PackagePrice));
                else
                    pricingInfo.Prices.Sort((x, y) => y.PackagePrice.CompareTo(x.PackagePrice));
            }

            foreach (Price p in pricingInfo.Prices)
            {
                Product prod = prods.Products.Find(x => x.ItemNumber == p.ItemNumber);
                prod.CasePrice = String.Format("{0:C}", p.CasePrice);
                prod.PackagePrice = String.Format("{0:C}", p.PackagePrice);
            }
        }
    }
}
