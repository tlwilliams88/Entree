using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.Catalog;
using KeithLink.Svc.Impl.Repository;

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
            //builder.Register(c => new StubCatalogRepositoryImpl()).As<ICatalogRepository>().InstancePerRequest();
            //builder.Register(c => new CommerceServerCatalogRepositoryImpl()).As<ICatalogRepository>().InstancePerRequest();
            builder.Register(p => new PriceRepositoryImpl()).As<IPriceRepository>().InstancePerRequest();
            builder.Register(c => new CatalogElasticSearchRepositoryImpl()).As<ICatalogRepository>().InstancePerRequest();

            builder.RegisterType<MockListRepositoryImpl>().As<IListRepository>().InstancePerRequest() ;

            // Build the container.
            var container = builder.Build();

            // Create the depenedency resolver.
            var resolver = new AutofacWebApiDependencyResolver(container);
            return resolver;
        }
    }
}