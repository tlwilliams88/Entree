using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;

namespace KeithLink.Svc.WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //******************************************************
            // moved to App_Start/WebApiConfig
            //******************************************************
            //GlobalConfiguration.Configure(WebApiConfig.Register);

            // Configure Web API with the dependency resolver.
            //var resolver = DependencyMap.Build();
            //GlobalConfiguration.Configuration.DependencyResolver = resolver;

            GlobalConfiguration.Configuration.EnsureInitialized(); 
        }
    }
}