using KeithLink.Svc.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace KeithLink.Svc.WebApi.Attribute
{
	public class AuthorizationAttribute: AuthorizeAttribute
	{
		internal string[] UserRoles { get; set; }

		public AuthorizationAttribute(params string[] allowedRoles) { UserRoles = allowedRoles; }
		
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