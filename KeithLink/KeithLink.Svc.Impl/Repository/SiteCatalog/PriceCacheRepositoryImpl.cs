using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.SiteCatalog
{
    public class PriceCacheRepositoryImpl : KeithLink.Svc.Core.IPriceCacheRepository
    {
        #region attributes
        private const string CACHE_GROUPNAME = "Pricing";
        private const string CACHE_NAME = "Pricing";
        private const string CACHE_PREFIX = "Default";

        private CommerceServer.Foundation.ICacheProvider cache;
        #endregion

        #region ctor
        public PriceCacheRepositoryImpl()
        {
            var cacheContext = CommerceServer.Foundation.PresentationCacheSettings.GetCacheContext(CACHE_GROUPNAME);
            cache = CommerceServer.Foundation.PresentationTypeLoader.CacheProvider(cacheContext);
        }
        #endregion

        #region methods
        /// <summary>
        /// add a price to the cache
        /// </summary>
        /// <param name="branchId">the branch's unique identifier</param>
        /// <param name="customerNumber">the customer's unique identifier</param>
        /// <param name="itemNumber">the item's unique identifier</param>
        /// <param name="casePrice">the customer's case price for the item</param>
        /// <param name="packagePrice">the customer's package price for the item</param>
        /// <remarks>
        /// jwames - 7/22/2014 - original code
        /// </remarks>
        public void AddItem(string branchId, string customerNumber, string itemNumber, double casePrice, double packagePrice)
        {
            cache.AddData<KeithLink.Svc.Core.Price>(CACHE_PREFIX,
                                                    CACHE_NAME,
                                                    GetCacheKey(branchId, customerNumber, itemNumber),
                                                    new Core.Price()
                                                    {
                                                        BranchId = branchId,
                                                        CustomerNumber = customerNumber,
                                                        ItemNumber = itemNumber,
                                                        CasePrice = casePrice,
                                                        PackagePrice = packagePrice
                                                    },
                                                    GetCacheExpiration()
                                                    );
        }

        /// <summary>
        /// add a price to the cache
        /// </summary>
        /// <param name="price">the pricing object for the item to cache</param>
        /// <remarks>
        /// jwames - 7/28/2014 - original code
        /// </remarks>
        public void AddItem(Core.Price price)
        {
            cache.AddData<KeithLink.Svc.Core.Price>(CACHE_PREFIX,
                                                    CACHE_NAME,
                                                    GetCacheKey(price.BranchId, price.CustomerNumber, price.ItemNumber),
                                                    price,
                                                    GetCacheExpiration()
                                                    );
        }

        //private CommerceServer.Foundation.ICacheProvider GetCacheProvider()
        //{
        //    var cacheContext = CommerceServer.Foundation.PresentationCacheSettings.GetCacheContext(CACHE_GROUPNAME);
        //    var cacheProvider = CommerceServer.Foundation.PresentationTypeLoader.CacheProvider(cacheContext);

        //    return cacheProvider;
        //}

        /// <summary>
        /// get the time that the cache will expire (06:00 the next day)
        /// </summary>
        /// <returns>
        /// jwames - 7/22/2014 - original code
        /// jwames - 7/28/2014 - correct the expiration logic
        /// </returns>
        private TimeSpan GetCacheExpiration()
        {
            DateTime tomorrow = DateTime.Now.AddDays(1);

            // expire tomorrow at 6:00 AM
            DateTime expire = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 6, 0, 0);

            return expire.Subtract(DateTime.Now);
        }

        /// <summary>
        /// build a delimited string for the cache key
        /// </summary>
        /// <param name="branchId">the branch's unique identifier</param>
        /// <param name="customerNumber">the customer's unique identifier</param>
        /// <param name="itemNumber">the item's unique identifier</param>
        /// <returns>a dash delimited string</returns>
        /// <remarks>
        /// jwames - 7/22/2014 - original code
        /// </remarks>
        private string GetCacheKey(string branchId, string customerNumber, string itemNumber)
        {
            return string.Format("{0}-{1}-{2}", branchId, customerNumber, itemNumber);
        }

        /// <summary>
        /// read the cache dictionary and try to find the specified price
        /// </summary>
        /// <param name="branchId">the branch's unique identifier</param>
        /// <param name="customerNumber">the customer's unique identifier</param>
        /// <param name="itemNumber">the item's unique identifier</param>
        /// <returns>a price object if found, otherwise returns null</returns>
        /// <remarks>
        /// jwames - 7/22/2014 - original code
        /// </remarks>
        public Core.Price GetPrice(string branchId, string customerNumber, string itemNumber)
        {
            Core.Price output = null;

            if (cache.TryGetData<Core.Price>(CACHE_PREFIX,
                                             CACHE_NAME,
                                             GetCacheKey(branchId, customerNumber, itemNumber),
                                             out output))
            {
                return output;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// remove all of the items from cache
        /// </summary>
        /// <remarks>
        /// jwames - 7/22/2014 - original code
        /// </remarks>
        public void ResetAllItems()
        {
            cache.ClearDataCache(CACHE_PREFIX, CACHE_NAME);
        }

        /// <summary>
        /// remove all of the cache for a specific customer (NOT IMPLEMENTED YET)
        /// </summary>
        /// <param name="branchId">the branch's unique identifier</param>
        /// <param name="customerNumber">the customer's unique identifier</param>
        /// <remarks>
        /// jwames - 7/28/2014 - original code
        /// </remarks>
        public void ResetItemsByCustomer(string branchId, string customerNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// remove a price from the cache
        /// </summary>
        /// <param name="branchId">the branch's unique identifier</param>
        /// <param name="customerNumber">the customer's unique identifier</param>
        /// <param name="itemNumber">the item's unique identifier</param>
        /// <remarks>
        /// jwames - 7/22/2014 - original code
        /// </remarks>
        public void RemoveItem(string branchId, string customerNumber, string itemNumber)
        {
            cache.RemoveData(CACHE_PREFIX,
                             CACHE_NAME,
                             GetCacheKey(branchId, customerNumber, itemNumber)
                             );
        }
        #endregion
    }
}
