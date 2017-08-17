using KeithLink.Svc.Core.Interface.Cache;

using CommerceServer.Foundation;

using System;
using System.Net.Http;

using KeithLink.Common.Core.Interfaces.Logging;

namespace KeithLink.Svc.Impl.Repository.Cache
{
	public class CacheRepositoryImpl: ICacheRepository
	{
        #region attributes
        private ICacheProvider _cache { get; set; }
        private IEventLogRepository _log { get; set; }
        #endregion

        #region constructor
	    public CacheRepositoryImpl(IEventLogRepository log) {
	        _log = log;
	    }
        #endregion

        #region methods
        public void AddItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key, TimeSpan timeout, T item) {
            CachingProvider(cacheGroupName).AddData<T>(cachePrefix, cacheName, key, item, timeout);
        }

        private ICacheProvider CachingProvider(string cacheGroupName) {
            if(_cache == null) {
                var cacheContext = PresentationCacheSettings.GetCacheContext(cacheGroupName);
                _cache = PresentationTypeLoader.CacheProvider(cacheContext);
            }
            return _cache;
        }

        public T GetItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key) {
            T output = default(T);
            if(CachingProvider(cacheGroupName).TryGetData<T>(cachePrefix,
                                             cacheName,
                                             key,
                                             out output)) {
                return output;
            } else {
                return default(T);
            }
        }

        public void RemoveItem(string cacheGroupName, string cachePrefix, string cacheName, string key) {
            //Trigger refresh item on all servers
            var servers = Configuration.CacheServersEndpoints;

            foreach(var server in servers) {
                using(HttpClient client = new HttpClient()) {
                    try {
                        var r = client.GetAsync(string.Format("{0}/Cache/RefreshCacheItem?cacheGroupName={1}&cachePrefix={2}&cacheName={3}&key={4}", server, cacheGroupName, cachePrefix, cacheName, key))
                                      .Result;
                        if (r.IsSuccessStatusCode != true) {
                            _log.WriteErrorLog(string.Format("Cache endpoint {0} returned a status code {1}. Message: {3}", server, r.StatusCode, r.Content.ReadAsStringAsync()
                                                                                                                                                   .Result.ToString()));
                        }
                    } catch (Exception ex) {
                        _log.WriteErrorLog("Cache endpoint failed.", ex);
                    }//Log?
                }
            }
        }

        public void ResetAllItems(string cacheGroupName, string cachePrefix, string cacheName) {
            //Trigger refresh all items on all servers
            var servers = Configuration.CacheServersEndpoints;

            foreach(var server in servers) {
                using(HttpClient client = new HttpClient()) {
                    try {
                        var r = client.GetAsync(string.Format("{0}/Cache/RefreshCache?cacheGroupName={1}&cachePrefix={2}&cacheName={3}", server, cacheGroupName, cachePrefix, cacheName))
                                      .Result;
                        if (r.IsSuccessStatusCode != true) {
                            _log.WriteErrorLog(string.Format("Cache endpoint {0} returned a status code {1}. Message: {3}", server, r.StatusCode, r.Content.ReadAsStringAsync()
                                                                                                                                                   .Result.ToString()));
                        }
                    } catch (Exception ex) {
                        _log.WriteErrorLog("Cache endpoint failed.", ex);
                    }
                }
            }
        }
        #endregion
    }
}
