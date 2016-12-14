﻿using KeithLink.Svc.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace KeithLink.Svc.WebApi.Attribute
{
    /// <summary>
    /// AddCustomHeaderAttribute
    /// </summary>
	public class AddCustomHeaderAttribute : ActionFilterAttribute
	{
        /// <summary>
        /// OnActionExecuted for AddCustomHeaderAttribute
        /// </summary>
        /// <param name="actionExecutedContext"></param>
		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			if(Configuration.AddServerNameToHeader)
				try
				{
					actionExecutedContext.Response.Content.Headers.Add("serverName", System.Environment.MachineName.ToString());
				}catch{}
			try
			{
				actionExecutedContext.Response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue()
				{
					MaxAge = new TimeSpan(0),
					NoCache = true,
					NoStore = true
				};
			}
			catch { }
		}
	}
}