using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl.Repository.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Lists
{
	public class ListCachRepositoryImpl: CacheRepositoryBase, IListCacheRepository
    {
        #region attributes
        protected override string CACHE_GROUPNAME { get { return "UserList"; } }
        protected override string CACHE_NAME { get { return "UserList"; } }
        protected override string CACHE_PREFIX { get { return "Default"; } }
        #endregion

        public void AddItem<T>(string key, T item)
        {
            cache.AddData<T>(CACHE_PREFIX, CACHE_NAME, key, item, TimeSpan.FromHours(2));
        }

        public void ResetAllItems()
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(string key)
        {
            cache.RemoveData(CACHE_PREFIX,
                 CACHE_NAME,
                 key
                 );
        }

        public T GetItem<T>(string key)
        {
            T output = default(T);
            if (cache.TryGetData<T>(CACHE_PREFIX,
                                             CACHE_NAME,
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
    }
}
