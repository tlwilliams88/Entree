using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Impl.Repository.Cache;

namespace KeithLink.Svc.Impl.Repository.Profile.Cache
{
    public class NoCacheCustomerCacheRepositoryImpl : ICustomerCacheRepository
    {
        public void AddItem<T>(string key, T item)
        {
                    }

        public void ResetAllItems()
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(string key)
        {
        }

        public T GetItem<T>(string key)
        {
            return default(T);
        }
    }
}
