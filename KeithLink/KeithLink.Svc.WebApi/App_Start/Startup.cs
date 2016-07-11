﻿using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Swashbuckle.Application;
using System.Reflection;

[assembly: OwinStartup(typeof(KeithLink.Svc.WebApi.Startup))]
namespace KeithLink.Svc.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureOAuth(app);


			//Only enable swagger for local environments
#if DEV || DEBUG 

			config
				.EnableSwagger(c =>
				{
					c.SingleApiVersion("v1", "Entree Api");
					c.IncludeXmlComments(GetXmlCommentsPath());
					//c.ResolveConflictingActions(apiDescriptions => apiDescriptions.FirstOrDefault());
				})
				.EnableSwaggerUi(c =>
				{
					c.InjectStylesheet(Assembly.GetExecutingAssembly(), "SwaggerUI.css");
				});
#endif
			
			
			WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        /// <summary>
        /// sets all of the options for the OAuth tokens
        /// </summary>
        /// <param name="app"></param>
        /// <remarks>
        /// jwames - 5/29/2015 - change the duration of the token to 7 days
        /// </remarks>
        public void ConfigureOAuth(IAppBuilder app) {
            OAuthAuthorizationServerOptions serverOptions = new OAuthAuthorizationServerOptions() {
                AllowInsecureHttp = !KeithLink.Svc.Impl.Configuration.RequireHttps,
                TokenEndpointPath = new PathString("/authen"),
                //AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(KeithLink.Svc.Impl.Configuration.LoginTokenDuration),
                Provider = new SimpleAuthorizationServerProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(serverOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }
        
        protected static string GetXmlCommentsPath()
		{
			return System.String.Format(@"{0}\bin\KeithLink.Svc.WebApi.XML", System.AppDomain.CurrentDomain.BaseDirectory);
		}


    }

	

}