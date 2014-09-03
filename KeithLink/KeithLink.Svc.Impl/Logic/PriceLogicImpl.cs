using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic
{
    public class PriceLogicImpl : IPriceLogic
    {
        #region attributes
        IPriceRepository _priceRepository;
        IPriceCacheRepository _priceCacheRepository;
        #endregion

        public PriceLogicImpl(IPriceRepository priceRepository, IPriceCacheRepository priceCacheRepository)
        {
            _priceRepository = priceRepository;
            _priceCacheRepository = priceCacheRepository;
        }

        public Core.Models.SiteCatalog.PriceReturn GetPrices(string BranchId, string customerNumber, DateTime shipDate, List<Core.Models.SiteCatalog.Product> products)
        {
            List<Price> cachedPriceList = null;
            List<Product> uncachedProductList = null;

            BuildCachedPriceList(BranchId, customerNumber, products, out cachedPriceList, out uncachedProductList);

            PriceReturn retVal = new PriceReturn();

            retVal.Prices.AddRange(cachedPriceList);
            if (uncachedProductList.Count > 0)
            {
                List<Price> uncachedPrices = _priceRepository.GetPrices(BranchId, customerNumber, shipDate, uncachedProductList);
                foreach (Price p in uncachedPrices)
                {
                    _priceCacheRepository.AddItem(p);
                }
                retVal.Prices.AddRange(cachedPriceList);
            }
            return retVal;
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
        private void BuildCachedPriceList(string branchId, string customerNumber, List<Product> fullList,
                                          out List<Price> cachedList, out List<Product> newProductList)
        {
            cachedList = new List<Price>();
            newProductList = new List<Product>();

            foreach (Product currentProduct in fullList)
            {
                Price tempPrice = _priceCacheRepository.GetPrice(branchId, customerNumber, currentProduct.ItemNumber);

                if (tempPrice == null)
                {
                    newProductList.Add(currentProduct);
                }
                else
                {
                    cachedList.Add(tempPrice);
                }
            }
        }
    }
}
