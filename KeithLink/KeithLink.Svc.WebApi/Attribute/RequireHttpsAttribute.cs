using KeithLink.Svc.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace KeithLink.Svc.WebApi.Attribute
{
	public class RequireHttpsAttribute: ActionFilterAttribute
	{
		private bool allowHttp { get; set; }

		public RequireHttpsAttribute()
		{
			this.allowHttp = false;
		}

		public RequireHttpsAttribute(bool allowHttp)
		{
			this.allowHttp = allowHttp;
		}

		public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			if (Configuration.RequireHttps && !allowHttp && actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
			{
				actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
			}
		}		
	}
}