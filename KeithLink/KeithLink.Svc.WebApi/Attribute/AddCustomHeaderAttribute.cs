using KeithLink.Svc.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace KeithLink.Svc.WebApi.Attribute
{
	public class AddCustomHeaderAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			if(Configuration.AddServerNameToHeader)
				actionExecutedContext.Response.Content.Headers.Add("serverName", System.Environment.MachineName.ToString());

		}
	}
}