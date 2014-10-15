using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Impl.Repository.Cache;

namespace KeithLink.Svc.Impl.Repository.Profile.Cache
{
    public class CustomerCacheRepositoryImpl : CacheRepositoryBase, ICustomerCacheRepository
    {
        #region attributes
        protected override string CACHE_GROUPNAME { get { return "Profile"; } }
        protected override string CACHE_NAME { get { return "Profile"; } }
        protected override string CACHE_PREFIX { get { return "Default"; } }
        #endregion

        public void AddItem<T>(string key, T item)
        {
            cache.AddData<T>(CACHE_PREFIX, CACHE_NAME, key, item, TimeSpan.FromHours(4));
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
