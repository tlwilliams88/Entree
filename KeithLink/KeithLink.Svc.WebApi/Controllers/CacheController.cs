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

		/// <summary>
		/// Used for clearing an item from the WebAPI cache
		/// </summary>
		/// <param name="cacheGroupName">Cache Group</param>
		/// <param name="cachePrefix">Cache Prefix</param>
		/// <param name="cacheName">Cache Name</param>
		/// <param name="key">Cache Key</param>
		[Route("cache/RefreshCacheItem")]
		[HttpGet]
		public void RefreshCacheItem(string cacheGroupName, string cachePrefix, string cacheName, string key)
		{
			cacheRepository.RefreshCacheItem(cacheGroupName, cachePrefix, cacheName, key);
		}

		/// <summary>
		/// Clears all items from a WebAPI cache group
		/// </summary>
		/// <param name="cacheGroupName">Cache Group</param>
		/// <param name="cachePrefix">Cache Prefix</param>
		/// <param name="cacheName">Cache Name</param>
		[Route("cache/RefreshCache")]
		[HttpGet]
		public void RefreshCache(string cacheGroupName, string cachePrefix, string cacheName)
		{
			cacheRepository.RefreshCache(cacheGroupName, cachePrefix, cacheName);
		}
	}
}