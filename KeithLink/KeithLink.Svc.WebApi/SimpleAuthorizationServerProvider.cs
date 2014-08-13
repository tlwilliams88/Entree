using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi
{
    public class SimpleAuthorizationServerProvider : Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerProvider
    {
        #region
        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(Microsoft.Owin.Security.OAuth.OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            Impl.Profile.UserProfileRepository userRepo = new Impl.Profile.UserProfileRepository();

            Core.Profile.UserProfileReturn userProfile = null;

            bool success = userRepo.AuthenticateUser(context.UserName, context.Password, out userProfile);

            if (!success)
            {
                context.SetError("invalid_grant", "The use name or password is incorrect.");
                return;
            }

            var identity = new System.Security.Claims.ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new System.Security.Claims.Claim("name", context.UserName));
            identity.AddClaim(new System.Security.Claims.Claim("role", "Owner"));

            context.Validated(identity);
        }

        public override async System.Threading.Tasks.Task ValidateClientAuthentication(Microsoft.Owin.Security.OAuth.OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        #endregion
    }
}