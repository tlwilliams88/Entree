using KeithLink.Svc.Core.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Common.Core.Logging;

namespace KeithLink.Svc.Impl.Repository.Profile
{
    public class UserProfileCacheRepository : IUserProfileCacheRepository
    {
        #region attributes
        private static object cacheLock = new object();
        private const string CACHE_GROUPNAME = "Profile";
        private const string CACHE_NAME = "Profile";
        private const string CACHE_PREFIX = "Default";

        private CommerceServer.Foundation.ICacheProvider cache;
        #endregion

        #region ctor
        public UserProfileCacheRepository()
        {
            var cacheContext = CommerceServer.Foundation.PresentationCacheSettings.GetCacheContext(CACHE_GROUPNAME);
            cache = CommerceServer.Foundation.PresentationTypeLoader.CacheProvider(cacheContext);
        }
        #endregion

        #region methods
        /// <summary>
        /// add a price to the cache
        /// </summary>
        /// <param name="price">the pricing object for the item to cache</param>
        /// <remarks>
        /// jwames - 7/28/2014 - original code
        /// </remarks>
        public void AddProfile(Core.Models.Profile.UserProfile userProfile)
        {
            lock (cacheLock)
            {
                if (GetProfile(userProfile.UserName) == null)
                {
                    cache.AddData<KeithLink.Svc.Core.Models.Profile.UserProfile>(CACHE_PREFIX,
                                                            CACHE_NAME,
                                                            GetCacheKey(userProfile.UserName),
                                                            userProfile,
                                                            GetCacheExpiration()
                                                            );
                }
            }
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
            return new TimeSpan(0, 1, 0); // TODO - tune this and ensure chache is refreshed when changes occur!
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
        private string GetCacheKey(string emailAddress)
        {
            return string.Format("{0}-{1}", "bek", emailAddress);
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
        public Core.Models.Profile.UserProfile GetProfile(string emailAddress)
        {
            Core.Models.Profile.UserProfile output = null;

            if (cache.TryGetData<Core.Models.Profile.UserProfile>(CACHE_PREFIX,
                                             CACHE_NAME,
                                             GetCacheKey(emailAddress),
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
            lock (cacheLock)
            {
                cache.ClearDataCache(CACHE_PREFIX, CACHE_NAME);
            }
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
        public void RemoveItem(string emailAddress)
        {
            lock (cacheLock)
            {
                cache.RemoveData(CACHE_PREFIX,
                                 CACHE_NAME,
                                 GetCacheKey(emailAddress)
                                 );
            }
        }
        #endregion
    }
}
