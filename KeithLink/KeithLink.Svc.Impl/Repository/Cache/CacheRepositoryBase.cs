using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Cache
{
    public abstract class CacheRepositoryBase
    {
        #region attributes
        protected abstract string CACHE_GROUPNAME { get; }
        protected abstract string CACHE_NAME { get; }
        protected abstract string CACHE_PREFIX { get; }

        protected CommerceServer.Foundation.ICacheProvider cache;
        #endregion

        #region ctor
        protected CacheRepositoryBase()
        {
            var cacheContext = CommerceServer.Foundation.PresentationCacheSettings.GetCacheContext(CACHE_GROUPNAME);
            cache = CommerceServer.Foundation.PresentationTypeLoader.CacheProvider(cacheContext);
        }
        #endregion
    }
}
