using KeithLink.Svc.Core.Interface.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Cache
{
	public class CacheRepositoryImpl: ICacheRepository
	{
		private CommerceServer.Foundation.ICacheProvider _cache { get; set; }

		
				
		public void AddItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key, TimeSpan timeout, T item)
		{
			CachingProvider(cacheGroupName).AddData<T>(cachePrefix, cacheName, key, item, timeout);
		}
				
		public T GetItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key)
		{
			T output = default(T);
			if (CachingProvider(cacheGroupName).TryGetData<T>(cachePrefix,
											 cacheName,
											 key,
											 out output))
			{
				return output;
			}
			else
			{
				return default(T);
			}
		}

		public void ResetAllItems(string cacheGroupName, string cachePrefix, string cacheName)
		{
			//Trigger refresh all items on all servers
			var servers = Configuration.CacheServersEndpoints;

			foreach (var server in servers)
			{
				using (HttpClient client = new HttpClient())
				{
					try
					{
						var r = client.GetAsync(string.Format("{0}/Cache/RefreshCache?cacheGroupName={1}&cachePrefix={2}&cacheName={3}", server, cacheGroupName, cachePrefix, cacheName)).Result;
					}
					catch (Exception ex) { }
				}
			}
		}

		public void RemoveItem(string cacheGroupName, string cachePrefix, string cacheName, string key)
		{
            //CachingProvider(cacheGroupName).RemoveData(cachePrefix, cacheName, key);

            //Trigger refresh item on all servers
            var servers = Configuration.CacheServersEndpoints;

            foreach (var server in servers) {
                using (HttpClient client = new HttpClient()) {
                    try {
                        var r = client.GetAsync(string.Format("{0}/Cache/RefreshCacheItem?cacheGroupName={1}&cachePrefix={2}&cacheName={3}&key={4}", server, cacheGroupName, cachePrefix, cacheName, key)).Result;
                    } catch (Exception ex) { }//Log?
                }
            }
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
