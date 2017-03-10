using KeithLink.Svc.Core.Enumerations.Authentication;
using KeithLink.Svc.Core.Exceptions.Profile;

using KeithLink.Svc.Core.Interface.Profile;

using KeithLink.Svc.Core.Models.Authentication;
using KeithLink.Svc.Core.Models.Profile;

using KeithLink.Svc.Impl.Helpers;

using Microsoft.Owin.Security.OAuth;

using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace KeithLink.Svc.WebApi
{
    /// <summary>
    /// handles authentication for OAUTH2 via OWIN
    /// </summary>
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        #region attributes
        #endregion

        #region methods / functions
        /// <summary>
        /// handles the authentication of the user and creates the authentication token
        /// </summary>
        /// <returns>nothing</returns>
        /// <remarks>
        /// jwames - 8/12/2014 - original code
        /// </remarks>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            if (!ValidateApiKey(context))
                return;

            string errMsg = null;


            // determine if we are authenticating an internal or external user
            if (ProfileHelper.IsInternalAddress(context.UserName)) {
                IUserDomainRepository ADRepo = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserDomainRepository)) as IUserDomainRepository;

                bool success = await Task.Run<bool>(() => ADRepo.AuthenticateUser(context.UserName,
                                                                                  System.Web.HttpUtility.UrlDecode(context.Password), 
                                                                                  out errMsg));

                if (!success) {
                    context.SetError("invalid_grant", errMsg);
                    return;
                }
            } else {
                ICustomerDomainRepository ADRepo = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ICustomerDomainRepository)) as ICustomerDomainRepository;

                AuthenticationModel authentication = await Task.Run<AuthenticationModel>
                    (() => ADRepo.AuthenticateUser(context.UserName,
                                                   System.Web.HttpUtility.UrlDecode(context.Password)));

                if (!authentication.Status.Equals( AuthenticationStatus.Successful ) && !authentication.Status.Equals( AuthenticationStatus.PasswordExpired ) ) {
                    context.SetError( "invalid_grant", authentication.Message );
                    return;
                }
            }

            IUserProfileLogic _profileLogic = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserProfileLogic)) as IUserProfileLogic;
            UserProfileReturn userReturn = await Task.Run<UserProfileReturn>(() => _profileLogic.GetUserProfile(context.UserName));

            if (userReturn.UserProfiles.Count == 0) {
                context.SetError("invalid_grant", "User profile does not exist in Commerce Server");
            } else {
                _profileLogic.SetUserProfileLastLogin(userReturn.UserProfiles[0].UserId);
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                identity.AddClaim(new Claim("name", context.UserName));
                identity.AddClaim(new Claim("role", userReturn.UserProfiles[0].RoleName));

                context.Validated(identity);
            }
        }

        private static bool ValidateApiKey(OAuthGrantResourceOwnerCredentialsContext context)
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
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() => context.Validated());
        }

        #endregion
    }
}