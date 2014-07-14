﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Core;

namespace KeithLink.Svc.WebApi
{
    internal static class DependencyMap
    {
        public static AutofacWebApiDependencyResolver Build()
        {
            // Create the container builder.
            var builder = new ContainerBuilder();

            // Register the Web API controllers.
            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());

            // Register other dependencies.
            builder.Register(c => new StubCatalogRepositoryImpl()).As<ICatalogRepository>().InstancePerApiRequest();

            // Build the container.
            var container = builder.Build();

            // Create the depenedency resolver.
            var resolver = new AutofacWebApiDependencyResolver(container);
            return resolver;
        }
    }
}