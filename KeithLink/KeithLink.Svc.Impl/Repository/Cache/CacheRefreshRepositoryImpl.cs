using KeithLink.Svc.Core.Interface.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Cache
{
	public class CacheRefreshRepositoryImpl: ICacheRefreshRepository
	{

		private CommerceServer.Foundation.ICacheProvider _cache { get; set; }

		public void RefreshCacheItem(string cacheGroupName, string cachePrefix, string cacheName, string key)
		{
			CachingProvider(cacheGroupName).RemoveData(cachePrefix, cacheName, key);
		}

		public void RefreshCache(string cacheGroupName, string cachePrefix, string cacheName)
		{
			CachingProvider(cacheGroupName).ClearDataCache(cachePrefix, cacheName);
		}

		private CommerceServer.Foundation.ICacheProvider CachingProvider(string cacheGroupName)
		{
			if (_cache == null)
			{
				CommerceServer.Foundation.ICacheProvider cache;
				var cacheContext = CommerceServer.Foundation.PresentationCacheSettings.GetCacheContext(cacheGroupName);
				_cache = CommerceServer.Foundation.PresentationTypeLoader.CacheProvider(cacheContext);
			}
			return _cache;
		}		
	}
}
