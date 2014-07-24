using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl
{
    public class PriceCacheImpl : KeithLink.Svc.Core.IPriceCache
    {
        #region attributes
        private const string CACHE_GROUPNAME = "Pricing";
        private const string CACHE_NAME = "Pricing";
        private const string CACHE_PREFIX = "Default";

        private CommerceServer.Foundation.ICacheProvider cache;
        #endregion

        #region ctor
        public PriceCacheImpl()
        {
            var cacheContext = CommerceServer.Foundation.PresentationCacheSettings.GetCacheContext(CACHE_GROUPNAME);
            cache = CommerceServer.Foundation.PresentationTypeLoader.CacheProvider(cacheContext);
        }
        #endregion

        #region methods
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

        //private CommerceServer.Foundation.ICacheProvider GetCacheProvider()
        //{
        //    var cacheContext = CommerceServer.Foundation.PresentationCacheSettings.GetCacheContext(CACHE_GROUPNAME);
        //    var cacheProvider = CommerceServer.Foundation.PresentationTypeLoader.CacheProvider(cacheContext);

        //    return cacheProvider;
        //}

        private TimeSpan GetCacheExpiration()
        {
            DateTime tomorrow = DateTime.Now.AddDays(1);

            // expire tomorrow at 6:00 AM
            DateTime expire = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 6, 0, 0);

            return DateTime.Now.Subtract(expire);
        }

        private string GetCacheKey(string branchId, string customerNumber, string itemNumber)
        {
            return string.Format("{0}-{1}-{2}", branchId, customerNumber, itemNumber);
        }

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

        public void ResetAllItems()
        {
            cache.ClearDataCache(CACHE_PREFIX, CACHE_NAME);
        }

        public void ResetItemsByCustomer(string branchId, string customerNumber)
        {
            throw new NotImplementedException();
        }

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
