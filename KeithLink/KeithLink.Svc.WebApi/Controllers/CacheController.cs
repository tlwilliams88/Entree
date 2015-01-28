using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.WebApi.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace KeithLink.Svc.WebApi.Controllers
{
	[AllowAnonymous]
	[RequireHttps(true)]
	public class CacheController: BaseController
	{
		private ICacheRefreshRepository cacheRepository;

		public CacheController(ICacheRefreshRepository cacheRepository, IUserProfileLogic profileLogic): base(profileLogic)
		{
			this.cacheRepository = cacheRepository;
		}

		[Route("cache/RefreshCacheItem")]
		[HttpGet]
		public void RefreshCacheItem(string cacheGroupName, string cachePrefix, string cacheName, string key)
		{
			cacheRepository.RefreshCacheItem(cacheName, cachePrefix, cacheName, key);
		}
		[Route("cache/RefreshCache")]
		[HttpGet]
		public void RefreshCache(string cacheGroupName, string cachePrefix, string cacheName)
		{
			cacheRepository.RefreshCache(cacheName, cachePrefix, cacheName);
		}
	}
}