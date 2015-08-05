#region Using
using KeithLink.Common.Core.AuditLog;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;


using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Common.Impl.AuditLog;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.ContentManagement;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Impl.Logic.SiteCatalog;
using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.Cache;
using KeithLink.Svc.Impl.Repository.ContentManagement;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.InternalCatalog;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.OnlinePayments;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Queue;
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
			builder.RegisterType<UnitOfWorkMock>().As<IUnitOfWork>().InstancePerLifetimeScope(); ;

			//*******************************************
            //Logic Classes
			//*******************************************
			builder.RegisterType<InternalListLogic>().As<IInternalListLogic>();
			builder.RegisterType<SiteCatalogLogicImpl>().As<ICatalogLogic>();
			builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
			builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();
			builder.RegisterType<InternalDivisionLogic>().As<IInternalDivisionLogic>();
            builder.RegisterType<ContentManagementLogicImpl>().As<IContentManagementLogic>();
            builder.RegisterType<InternalDsrAliasLogicImpl>().As<IDsrAliasLogic>();
					
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
            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();

			//Etc
			builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
			builder.Register(c => new EventLogRepositoryImpl("Entree Test")).As<IEventLogRepository>();
			builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
			builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
            builder.RegisterType<ContentManagementExternalRepositoryImpl>().As<IContentManagementExternalRepository>();
            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();
            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
            
            //Profile
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();

			//Replace
			builder.RegisterType<NoOrderServiceRepositoryImpl>().As<IOrderServiceRepository>();
			builder.RegisterType<NoDivisionServiceRepositoryImpl>().As<IDivisionServiceRepository>();
			builder.RegisterType<NoListServiceRepositoryImpl>().As<IListServiceRepository>();
			builder.RegisterType<NoDsrServiceRepository>().As<IDsrServiceRepository>();
            builder.RegisterType<NoMessagingServiceRepositoryImpl>().As<IMessagingServiceRepository>();
            builder.RegisterType<NoInvoiceServiceRepositoryImpl>().As<IInvoiceServiceRepository>();
            builder.RegisterType<NoOnlinePaymentServiceRepository>().As<IOnlinePaymentServiceRepository>();
            builder.RegisterType<NoDsrAliasServiceImpl>().As<IDsrAliasService>();
            

            // Build the container.
            return builder.Build();

        }
    }
}