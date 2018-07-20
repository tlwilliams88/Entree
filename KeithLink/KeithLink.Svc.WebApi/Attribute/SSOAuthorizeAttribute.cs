using KeithLink.Svc.Impl;
using KeithLink.Svc.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace KeithLink.Svc.WebApi.Attribute
{
    /// <summary>
    /// AddCustomHeaderAttribute
    /// </summary>
	public class SSOAuthorizeAttribute : ActionFilterAttribute
	{
        /// <summary>
        /// OnActionExecuted for AddCustomHeaderAttribute
        /// </summary>
        /// <param name="actionContext"></param>
		public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Contains("ssoToken") &&
                actionContext.Request.Headers.Contains("username"))
            {
                var email = actionContext.Request.Headers.GetValues("username").First();

                string keyandemail = String.Concat(email.ToString(), Impl.Configuration.SSOSharedKey.ToString());

                SHA512 sha512 = SHA512.Create();
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(keyandemail);
                byte[] hash = sha512.ComputeHash(bytes);

                System.Text.StringBuilder authtoken = new System.Text.StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    authtoken.Append(hash[i].ToString("X2"));
                }

                var generatedToken = authtoken.ToString();

                var givenToken = actionContext.Request.Headers.GetValues("ssoToken").First();

                if (generatedToken.Equals(givenToken) == false)
                {
                    DenyAction(actionContext);
                }
            }
            else
            {
                DenyAction(actionContext);
            }
        }

        private static void DenyAction(HttpActionContext actionContext)
        {
            actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
        }
    }
}