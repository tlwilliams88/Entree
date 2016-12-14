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
    /// <summary>
    /// RequireHttpsAttribute
    /// </summary>
	public class RequireHttpsAttribute: ActionFilterAttribute
	{
		private bool allowHttp { get; set; }

        /// <summary>
        /// constructor for RequireHttpsAttribute
        /// </summary>
		public RequireHttpsAttribute()
		{
			this.allowHttp = false;
		}

        /// <summary>
        /// constructor for RequireHttpsAttribute that takes allowHttp
        /// </summary>
        /// <param name="allowHttp"></param>
		public RequireHttpsAttribute(bool allowHttp)
		{
			this.allowHttp = allowHttp;
		}

        /// <summary>
        /// OnActionExecuting for RequireHttpsAttribute
        /// </summary>
        /// <param name="actionContext"></param>
		public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			if (Configuration.RequireHttps && !allowHttp && actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
			{
				actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
			}
		}		
	}
}