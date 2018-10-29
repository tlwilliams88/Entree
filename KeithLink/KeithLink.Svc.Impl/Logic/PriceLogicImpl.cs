using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic
{
    public class PriceLogicImpl : IPriceLogic
    {
        #region attributes
        IPriceRepository _priceRepository;
        ICacheRepository _priceCacheRepository;

		private const string CACHE_GROUPNAME = "Pricing";
		private const string CACHE_NAME = "Pricing";
		private const string CACHE_PREFIX = "Default";
        #endregion

        #region ctor
        public PriceLogicImpl(IPriceRepository priceRepository, ICacheRepository priceCacheRepository) {
            _priceRepository = priceRepository;
            _priceCacheRepository = priceCacheRepository;
        }
        #endregion

        #region methods

        public PriceReturn GetPrices(string branchId, string customerNumber, DateTime shipDate, IEnumerable<Product> products)
        {
            return GetPrices(branchId, customerNumber, shipDate, products.AsQueryable<Product>());
        }

        public PriceReturn GetPrices(string branchId, string customerNumber, DateTime shipDate, IQueryable<Product> products)
        {
            List<Price> cachedPriceList = null;
            List<Product> uncachedProductList = null;

            BuildCachedPriceList(branchId, customerNumber, products, out cachedPriceList, out uncachedProductList);

            PriceReturn priceReturn = new PriceReturn();

            priceReturn.Prices.AddRange(cachedPriceList);

            if (uncachedProductList.Count > 0)
            {
                List<Price> uncachedPrices = _priceRepository.GetPrices(branchId, customerNumber, shipDate, uncachedProductList);

                uncachedPrices.ForEach(price => AddPriceToCache(price));

                priceReturn.Prices.AddRange(uncachedPrices);
            }
            return priceReturn;
        }

        public PriceReturn GetNonBekItemPrices(string branchId, string customerNumber, string source, DateTime shipDate, IEnumerable<Product> products)
        {
            return GetPrices(branchId, customerNumber, shipDate, products.AsQueryable<Product>());
        }

        public PriceReturn GetNonBekItemPrices(string branchId, string customerNumber, string source, DateTime shipDate, IQueryable<Product> products)
        {
            List<Price> cachedPriceList = null;
            List<Product> uncachedProductList = null;

            BuildCachedPriceList(branchId, customerNumber, products, out cachedPriceList, out uncachedProductList);

            PriceReturn priceReturn = new PriceReturn();

            priceReturn.Prices.AddRange(cachedPriceList);

            if (uncachedProductList.Count > 0)
            {
                List<Price> uncachedPrices = _priceRepository.GetNonBekItemPrices(branchId, customerNumber, shipDate, source, uncachedProductList);

                uncachedPrices.ForEach(price => AddPriceToCache(price));

                priceReturn.Prices.AddRange(uncachedPrices);
            }

            return priceReturn;
        }

        private string GetCacheKey(string branchId, string customerNumber, string itemNumber)
		{
			return string.Format("{0}-{1}-{2}", branchId, customerNumber, itemNumber);
		}
        
        /// <summary>
        /// separate items that are cached from non-cached items
        /// </summary>  
        /// <param name="branchId">the branch's unique identifier</param>
        /// <param name="customerNumber">the customer's unique identifier</param>
        /// <param name="fullList">the full list from the pricing request</param>
        /// <param name="cachedList">the list of prices that have been cached</param>
        /// <param name="newProductList">the list of products that have not been cached</param>
        /// <remarks>
        /// jwames - 7/28/2014 - original code
        /// </remarks>
        private void BuildCachedPriceList(string branchId, 
                                          string customerNumber, 
                                          IQueryable<Product> fullList,
                                          out List<Price> cachedList, 
                                          out List<Product> newProductList)
        {
            cachedList = new List<Price>();
            newProductList = new List<Product>();

            foreach (Product product in fullList)
            {
                // don't get prices for currentProducts that reference custominventory items
                if (product.CatalogId == null || product.CatalogId.Equals(Constants.CATALOG_CUSTOMINVENTORY, StringComparison.CurrentCultureIgnoreCase) == false)
                {
                    Price price = GetPriceFromCache(branchId, customerNumber, product);

                    if (price == null)
                    {
                        newProductList.Add(product);
                    }
                    else
                    {
                        cachedList.Add(price);
                    }
                }
            }
        }

        public Price GetPrice(string branchId, string customerNumber, DateTime shipDate, Product product)
        {
            Price price = null;

            // don't get prices for products that reference custominventory items
            if (product.CatalogId == null || product.CatalogId.Equals(Constants.CATALOG_CUSTOMINVENTORY, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                price = GetPriceFromCache(branchId, customerNumber, product);

                if (price == null)
                {
                    price = _priceRepository.GetPrices(branchId, customerNumber, shipDate, new List<Product> { product }).FirstOrDefault();
                    AddPriceToCache(price);
                }
            }

            return price;
        }

        private Price GetPriceFromCache(string branchId, string customerNumber, Product product)
        {
            string cacheKey = GetCacheKey(branchId, customerNumber, product.ItemNumber);
            Price price = _priceCacheRepository
                .GetItem<Price>(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, cacheKey);

            return price;
        }

        private void AddPriceToCache(Price price)
        {
            string cacheKey = GetCacheKey(price.BranchId, price.CustomerNumber, price.ItemNumber);
            _priceCacheRepository
                .AddItem(CACHE_GROUPNAME, CACHE_PREFIX, CACHE_NAME, cacheKey, TimeSpan.FromHours(2), price);
        }

        #endregion
    }
}
