using KeithLink.Svc.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace KeithLink.Svc.WebApi.Attribute
{
    /// <summary>
    /// AuthorizationAttribute
    /// </summary>
	public class AuthorizationAttribute: AuthorizeAttribute
	{
		internal string[] UserRoles { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="allowedRoles"></param>
		public AuthorizationAttribute(params string[] allowedRoles) { UserRoles = allowedRoles; }

        /// <summary>
        /// IsAuthorized for AuthorizationAttribute
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
		{
			if (HttpContext.Current.User.Identity.IsAuthenticated)
			{
				if (actionContext.ControllerContext.Controller is BaseController)
				{
					var user = ((BaseController)actionContext.ControllerContext.Controller).AuthenticatedUser;

					return UserRoles.Contains(user.RoleName);
				}
			}
			return base.IsAuthorized(actionContext);
		}		
	}
}