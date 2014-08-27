using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Impl.Repository.Brands;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl;

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
            builder.Register(b => new BrandRepositoryImpl()).As<IBrandRepository>().InstancePerRequest();
            builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();

            builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
            builder.RegisterType<ListLogicImpl>().As<IListLogic>();

            builder.RegisterType<Impl.Profile.CustomerContainerRepository>().As<Core.Interface.Profile.ICustomerContainerRepository>();
            builder.RegisterType<Impl.Profile.UserProfileRepository>().As<Core.Interface.Profile.IUserProfileRepository>();
            builder.RegisterType<Impl.Profile.ExternalUserDomainRepository>().As<Impl.Profile.ExternalUserDomainRepository>();
            builder.RegisterType<Impl.Profile.InternalUserDomainRepository>().As<Impl.Profile.InternalUserDomainRepository>();

            // Build the container.
            var container = builder.Build();

            // Create the depenedency resolver.
            var resolver = new AutofacWebApiDependencyResolver(container);
            return resolver;
        }
    }
}