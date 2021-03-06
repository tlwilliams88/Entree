﻿using KeithLink.Svc.Impl.Repository.SmartResolver;

using Autofac.Integration.WebApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Configuration;

namespace KeithLink.Svc.WebApi
{
    /// <summary>
    /// custom configuration for the web api project
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// register the http configuration
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config)
        {
            // Configure Web API with the dependency resolver.
            var diMap = DependencyMapFactory.GetWebApiContainer();
            diMap.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());

            var resolver = new AutofacWebApiDependencyResolver(diMap.Build());
            config.DependencyResolver = resolver;
            GlobalConfiguration.Configuration.DependencyResolver = resolver;


            // Web API configuration and services
            System.Web.Http.Cors.EnableCorsAttribute enableCors = new System.Web.Http.Cors.EnableCorsAttribute(
                KeithLink.Svc.Impl.Configuration.CorsEnabledDomains,
                KeithLink.Svc.Impl.Configuration.CorsEnabledHeaders,
                KeithLink.Svc.Impl.Configuration.CorsEnabledMethods);
            config.EnableCors(enableCors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Formatters.Clear();
            config.Formatters.Add(new System.Net.Http.Formatting.JsonMediaTypeFormatter());

            // from web sample
            var jsonFormatter = config.Formatters.OfType<System.Net.Http.Formatting.JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            jsonFormatter.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
        }
    }
}
