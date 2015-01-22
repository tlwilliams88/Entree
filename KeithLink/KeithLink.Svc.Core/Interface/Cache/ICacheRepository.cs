using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Cache
{
	public interface ICacheRepository
	{
		void AddItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key, TimeSpan timeout, T item);
		void ResetAllItems(string cacheGroupName, string cachePrefix, string cacheName);
		void RemoveItem(string cacheGroupName, string cachePrefix, string cacheName, string key);
		T GetItem<T>(string cacheGroupName, string cachePrefix, string cacheName, string key);		
	}
}
