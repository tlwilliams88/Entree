﻿using Entree.Core.Cache.Interfaces;

using CommerceServer.Foundation;

namespace Entree.Core.Cache.Repositories
{
	public class CacheRefreshRepositoryImpl: ICacheRefreshRepository
	{
        #region attributes
        private ICacheProvider _cache { get; set; }
        #endregion

        #region methods
        private ICacheProvider CachingProvider(string cacheGroupName) {
            if(_cache == null) {
                var cacheContext = PresentationCacheSettings.GetCacheContext(cacheGroupName);
                _cache = PresentationTypeLoader.CacheProvider(cacheContext);
            }

            return _cache;
        }

        public void RefreshCache(string cacheGroupName, string cachePrefix, string cacheName) {
            CachingProvider(cacheGroupName).ClearDataCache(cachePrefix, cacheName);
        }

        public void RefreshCacheItem(string cacheGroupName, string cachePrefix, string cacheName, string key) {
            CachingProvider(cacheGroupName).RemoveData(cachePrefix, cacheName, key);
        }
        #endregion
    }
}
