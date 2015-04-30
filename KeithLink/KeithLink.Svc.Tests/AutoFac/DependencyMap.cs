#region Using
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.ContentManagement;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Impl.Logic.SiteCatalog;
using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.Cache;
using KeithLink.Svc.Impl.Repository.ContentManagement;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.SiteCatalog;

using KeithLink.Svc.Test.Mock;

using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
#endregion

namespace KeithLink.Svc.Test
{
    internal static class DependencyMap
    {
        public static IContainer Build()
        {
            // Create the container builder.
            var builder = new ContainerBuilder();

			//*******************************************
			//Mock Items
			//*******************************************
			builder.RegisterType<UnitOfWorkMock>().As<IUnitOfWork>();

			//*******************************************
            //Logic Classes
			//*******************************************
			builder.RegisterType<InternalListLogic>().As<IInternalListLogic>();
			builder.RegisterType<SiteCatalogLogicImpl>().As<ICatalogLogic>();
			builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
			builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();
			builder.RegisterType<InternalDivisionLogic>().As<IInternalDivisionLogic>();
            builder.RegisterType<ContentManagementLogicImpl>().As<IContentManagementLogic>();
					
			//*******************************************
			//Repositories
			//*******************************************
			//EF
			builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
			builder.RegisterType<ListItemRepositoryImpl>().As<IListItemRepository>();
			builder.RegisterType<ListShareRepositoryImpl>().As<IListShareRepository>();
			builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();
			builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();

			//Catalog
			builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
			builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
			builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
			builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();

			//Etc
			builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
			builder.Register(c => new EventLogRepositoryImpl("Entree Test")).As<IEventLogRepository>();
			builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
			builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
            builder.RegisterType<ContentManagementExternalRepositoryImpl>().As<IContentManagementExternalRepository>();

			//Replace
			builder.RegisterType<NoOrderServiceRepositoryImpl>().As<IOrderServiceRepository>();
			builder.RegisterType<NoDivisionServiceRepositoryImpl>().As<IDivisionServiceRepository>();
			builder.RegisterType<NoListServiceRepositoryImpl>().As<IListServiceRepository>();
			builder.RegisterType<NoDsrServiceRepository>().As<IDsrServiceRepository>();

            // Build the container.
            return builder.Build();

        }
    }
}