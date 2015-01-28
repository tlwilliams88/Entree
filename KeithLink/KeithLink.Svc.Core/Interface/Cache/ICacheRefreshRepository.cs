using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Cache
{
	public interface ICacheRefreshRepository
	{
		void RefreshCacheItem(string cacheGroupName, string cachePrefix, string cacheName, string key);
		void RefreshCache(string cacheGroupName, string cachePrefix, string cacheName);
	}
}
