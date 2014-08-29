using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Impl.Repository.Brands;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Core.Interface.Orders;
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
            builder.Register(l => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
            builder.Register(pi => new ProductImageRepositoryImpl()).As<IProductImageRepository>().InstancePerRequest();
			
            builder.RegisterType<ListLogicImpl>().As<IListLogic>();

            builder.RegisterType<Impl.Repository.Profile.CustomerContainerRepository>().As<Core.Interface.Profile.ICustomerContainerRepository>();
            builder.RegisterType<Impl.Repository.Profile.UserProfileRepository>().As<Core.Interface.Profile.IUserProfileRepository>();
			builder.RegisterType<ShoppingCartLogicImpl>().As<IShoppingCartLogic>();
			builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
            builder.RegisterType<Impl.Repository.Profile.ExternalUserDomainRepository>().As<Impl.Repository.Profile.ExternalUserDomainRepository>();
            builder.RegisterType<Impl.Repository.Profile.InternalUserDomainRepository>().As<Impl.Repository.Profile.InternalUserDomainRepository>();

            // Build the container.
            var container = builder.Build();

            // Create the depenedency resolver.
            var resolver = new AutofacWebApiDependencyResolver(container);
            return resolver;
        }
    }
}