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
        }
    }
}
