using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi
{
    public class SimpleAuthorizationServerProvider : Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerProvider
    {
        #region attributes
        Core.Interface.Profile.IUserProfileRepository _userRepo;
        #endregion

        #region
        /// <summary>
        /// handles the authentication of the user and creates the authentication token
        /// </summary>
        /// <returns>nothing</returns>
        /// <remarks>
        /// jwames - 8/12/2014 - original code
        /// </remarks>
        public override async System.Threading.Tasks.Task GrantResourceOwnerCredentials(Microsoft.Owin.Security.OAuth.OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            string errMsg = null;

            Core.Interface.Profile.IUserProfileRepository _userRepo = 
                System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(Core.Interface.Profile.IUserProfileRepository))
                as Core.Interface.Profile.IUserProfileRepository;
            if (_userRepo.AuthenticateUser(context.UserName, context.Password, out errMsg) == false)
            {
                context.SetError("invalid_grant", errMsg);
                return;
            }

            var identity = new System.Security.Claims.ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, context.UserName));
            identity.AddClaim(new System.Security.Claims.Claim("name", context.UserName));
            identity.AddClaim(new System.Security.Claims.Claim("role", "Owner"));

            context.Validated(identity);
        }

        /// <summary>
        /// returns the validated status
        /// </summary>
        /// <remarks>
        /// jwames - 8/12/2014 - original code
        /// </remarks>
        public override async System.Threading.Tasks.Task ValidateClientAuthentication(Microsoft.Owin.Security.OAuth.OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        #endregion
    }
}