﻿using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.WebApi.Attribute;
using KeithLink.Svc.WebApi.Models;
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
        private readonly IEventLogRepository _elRepo;

        public CacheController(ICacheRefreshRepository cacheRepository, IUserProfileLogic profileLogic, IEventLogRepository elRepo) : base(profileLogic)
		{
			this.cacheRepository = cacheRepository;
            this._elRepo = elRepo;
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
		public OperationReturnModel<bool> RefreshCacheItem(string cacheGroupName, string cachePrefix, string cacheName, string key)
		{
            OperationReturnModel<bool> ret = new OperationReturnModel<bool>();
            try
            {
                cacheRepository.RefreshCacheItem(cacheGroupName, cachePrefix, cacheName, key);
                ret = new OperationReturnModel<bool>() { SuccessResponse = true, IsSuccess = true };
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("RefreshCacheItem", ex);
            }
            return ret;
		}

		/// <summary>
		/// Clears all items from a WebAPI cache group
		/// </summary>
		/// <param name="cacheGroupName">Cache Group</param>
		/// <param name="cachePrefix">Cache Prefix</param>
		/// <param name="cacheName">Cache Name</param>
		[Route("cache/RefreshCache")]
		[HttpGet]
		public OperationReturnModel<bool> RefreshCache(string cacheGroupName, string cachePrefix, string cacheName)
		{
            OperationReturnModel<bool> ret = new OperationReturnModel<bool>();
            try
            {
                cacheRepository.RefreshCache(cacheGroupName, cachePrefix, cacheName);
                ret = new OperationReturnModel<bool>() { SuccessResponse = true, IsSuccess = true };
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrorMessage = ex.Message;
                _elRepo.WriteErrorLog("RefreshCache", ex);
            }
            return ret;
        }
    }
}