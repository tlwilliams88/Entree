using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.InternalSvc.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KeithLink.Svc.InternalSvc
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CacheService" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select CacheService.svc or CacheService.svc.cs at the Solution Explorer and start debugging.
	public class CacheService : ICacheService
	{
		private ICacheRefreshRepository cacheRepository;

		public CacheService(ICacheRefreshRepository cacheRepository)
		{
			this.cacheRepository = cacheRepository;
		}

		public void RefreshCacheItem(string cacheGroupName, string cachePrefix, string cacheName, string key)
		{
			cacheRepository.RefreshCacheItem(cacheName, cachePrefix, cacheName, key);
		}

		public void RefreshCache(string cacheGroupName, string cachePrefix, string cacheName)
		{
			cacheRepository.RefreshCache(cacheName, cachePrefix, cacheName);
		}
	}
}
