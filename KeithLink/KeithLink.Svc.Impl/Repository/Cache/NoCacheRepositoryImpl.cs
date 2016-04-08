using KeithLink.Svc.Core.Interface.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Cache
{
    /// <summary>
    /// Used for cases (like the services) where caching can't be used
    /// </summary>
    public class NoCacheRepositoryImpl : ICacheRepository
    {
        public void AddItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key, TimeSpan timeout, T item)
        {
            //throw new NotImplementedException();
        }

        public T GetItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key)
        {
            return default(T);
        }

        public void RemoveItem(string cacheGroupName, string cachePrefix, string cacheName, string key)
        {
            //throw new NotImplementedException();
        }

        public void ResetAllItems(string cacheGroupName, string cachePrefix, string cacheName)
        {
            //throw new NotImplementedException();
        }
    }
}
