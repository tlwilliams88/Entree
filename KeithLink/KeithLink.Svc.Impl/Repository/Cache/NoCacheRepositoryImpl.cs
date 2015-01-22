using KeithLink.Svc.Core.Interface.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Cache
{
	public class NoCacheRepositoryImpl: ICacheRepository
	{
		public void AddItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key, TimeSpan timeout, T item)
		{
			
		}

		public void ResetAllItems(string cacheGroupName, string cachePrefix, string cacheName)
		{
			
		}

		public void RemoveItem(string cacheGroupName, string cachePrefix, string cacheName, string key)
		{
			
		}

		public T GetItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key)
		{
			return default(T);
		}
	}
}
