using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Configuration;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //******************************************************
            // moved from Global.asax
            //******************************************************
            //GlobalConfiguration.Configure(WebApiConfig.Register);

            // Configure Web API with the dependency resolver.
            //var resolver = DependencyMap.Build();
            //GlobalConfiguration.Configuration.DependencyResolver = resolver;

            //GlobalConfiguration.Configuration.EnsureInitialized();
            //******************************************************
            // end of move from Global.asax
            //******************************************************

            // Configure Web API with the dependency resolver.
            var resolver = DependencyMap.Build();
            config.DependencyResolver = resolver;


            // Web API configuration and services
            System.Web.Http.Cors.EnableCorsAttribute enableCors = new System.Web.Http.Cors.EnableCorsAttribute(
                KeithLink.Svc.Impl.Configuration.CorsEnabledDomains,
                KeithLink.Svc.Impl.Configuration.CorsEnabledHeaders,
                KeithLink.Svc.Impl.Configuration.CorsEnabledMethods);
            config.EnableCors(enableCors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Clear();
            config.Formatters.Add(new System.Net.Http.Formatting.JsonMediaTypeFormatter());

            // from web sample
            var jsonFormatter = config.Formatters.OfType<System.Net.Http.Formatting.JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        }
    }
}
