using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using KeithLink.Svc.Core.Exceptions.Profile;

namespace KeithLink.Svc.WebApi
{
    public class SimpleAuthorizationServerProvider : Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerProvider
    {
        #region attributes
        Core.Interface.Profile.IUserProfileRepository _userRepo;
        #endregion

        #region methods / functions
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
            if (!ValidateApiKey(context))
                return;

            string errMsg = null;

            Core.Interface.Profile.IUserProfileLogic _profileLogic =
                System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(Core.Interface.Profile.IUserProfileLogic))
                as Core.Interface.Profile.IUserProfileLogic;

            // determine if we are authenticating an internal or external user
            if (_profileLogic.IsInternalAddress(context.UserName)) {
                Core.Interface.Profile.IUserDomainRepository ADRepo;
                ADRepo = System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(Core.Interface.Profile.IUserDomainRepository))
                as Core.Interface.Profile.IUserDomainRepository;

                if (ADRepo.AuthenticateUser(context.UserName, context.Password, out errMsg) == false) {
                    context.SetError("invalid_grant", errMsg);
                    return;
                }
            } else {
                Core.Interface.Profile.ICustomerDomainRepository ADRepo =
                    System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(Core.Interface.Profile.ICustomerDomainRepository))
                    as Core.Interface.Profile.ICustomerDomainRepository;

                if (ADRepo.AuthenticateUser(context.UserName, context.Password, out errMsg) == false) {
                    context.SetError("invalid_grant", errMsg);
                    return;
                }
            }


            //KeithLink.Svc.Core.Models.Profile.UserProfileReturn userReturn = _userRepo.GetUserProfile(context.UserName);
            KeithLink.Svc.Core.Models.Profile.UserProfileReturn userReturn = _profileLogic.GetUserProfile(context.UserName);

            var identity = new System.Security.Claims.ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, context.UserName));
            identity.AddClaim(new System.Security.Claims.Claim("name", context.UserName));
            identity.AddClaim(new System.Security.Claims.Claim("role", userReturn.UserProfiles[0].RoleName));

            context.Validated(identity);
        }

        private static bool ValidateApiKey(Microsoft.Owin.Security.OAuth.OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.Request.Body.Position = 0;
            byte[] bodyBytes = new byte[context.Request.Body.Length];
            context.Request.Body.Read(bodyBytes, 0, (int)context.Request.Body.Length);
            string body = System.Text.Encoding.UTF8.GetString(bodyBytes);

            if (!String.IsNullOrEmpty(body) && body.Contains("apiKey"))
            {
                NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(body);
                if (String.IsNullOrEmpty(queryString["apiKey"])
					|| !Svc.Impl.Configuration.AllowedApiKeys.Contains(queryString["apiKey"]))
                {
                    context.SetError("invalid_invalidkeyprovided", "Invalid Client Version - Please Update");
                    return false;
                }
            }
            else
            {
                context.SetError("invalid_nokeyprovided", "Invalid Client Version - Please Update");
                return false;
            }
            return true;
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