﻿#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Integration.WebApi;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Logic.SiteCatalog;
using KeithLink.Svc.Impl.Repository.Brands;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Customer;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Profile.Cache;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Configuration;
using KeithLink.Svc.Core.Interface.Export;
using KeithLink.Svc.Impl.Logic.Export;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Core.Interface.Component;
#endregion

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
            builder.Register(c => new ElasticSearchCatalogRepositoryImpl()).As<ICatalogRepository>().InstancePerRequest();
            builder.Register(b => new BrandRepositoryImpl()).As<IBrandRepository>().InstancePerRequest();
            builder.Register(l => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
            builder.Register(pi => new ProductImageRepositoryImpl()).As<IProductImageRepository>().InstancePerRequest();

            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<PriceCacheRepositoryImpl>().As<IPriceCacheRepository>();
            builder.RegisterType<SiteCatalogLogicImpl>().As<ICatalogLogic>();
			builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
            builder.RegisterType<AvatarRepositoryImpl>().As<IAvatarRepository>();
			builder.RegisterType<ShoppingCartLogicImpl>().As<IShoppingCartLogic>();
			builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
			builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();
			builder.RegisterType<OrderQueueRepositoryImpl>().As<IOrderQueueRepository>();
            builder.RegisterType<UserProfileCacheRepository>().As<IUserProfileCacheRepository>();
			builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();
            builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
            builder.RegisterType<CatalogCacheRepositoryImpl>().As<ICatalogCacheRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<ShipDateRepositoryImpl>().As<IShipDateRepository>();
            builder.RegisterType<OrderLogicImpl>().As<IOrderLogic>();
            builder.RegisterType<CustomerCacheRepositoryImpl>().As<ICustomerCacheRepository>();
			builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
			builder.RegisterType<OrderQueueLogicImpl>().As<IOrderQueueLogic>();
			builder.RegisterType<OrderQueueRepositoryImpl>().As<IOrderQueueRepository>();
			builder.RegisterType<OrderQueueRepositoryImpl>().As<IQueueRepository>();
			builder.RegisterType<ImportLogicImpl>().As<IImportLogic>();
			builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<ISocketConnectionRepository>();
            builder.RegisterType<OrderHistoryRequestLogicImpl>().As<IOrderHistoryRequestLogic>();
            builder.RegisterType<OrderUpdateRequestQueueRepositoryImpl>().As<IOrderHistoryRequestQueueRepository>();
			builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
			builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
			builder.RegisterType<TokenReplacer>().As<ITokenReplacer>();

            builder.RegisterType<KeithLink.Svc.Impl.Repository.Orders.History.EF.OrderHistoyrHeaderRepositoryImpl>().As<KeithLink.Svc.Core.Interface.Orders.History.IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<KeithLink.Svc.Impl.Repository.EF.Operational.UnitOfWork>().As<KeithLink.Svc.Impl.Repository.EF.Operational.IUnitOfWork>();

			builder.RegisterType<Repository.Lists.ListServiceRepositoryImpl>().As<IListServiceRepository>();
			builder.RegisterType<Repository.SiteCatalog.DivisionServiceRepositoryImpl>().As<IDivisionServiceRepository>();
            builder.RegisterType<Repository.Messaging.MessagingServiceRepositoryImpl>().As<IMessagingServiceRepository>();
            builder.RegisterType<Repository.OnlinePayments.OnlinePaymentServiceRepositoryImpl>().As<IOnlinePaymentServiceRepository>();
			builder.RegisterType<Repository.Orders.OrderServiceRepositoryImpl>().As<IOrderServiceRepository>();
            builder.RegisterType<Repository.Invoices.InvoiceServiceRepositoryImpl>().As<IInvoiceServiceRepository>();
			builder.RegisterType<Repository.Configurations.ExportSettingServiceRepositoryImpl>().As<IExportSettingServiceRepository>();
            builder.RegisterType<Repository.Reports.ReportServiceRepositoryImpl>().As<IReportServiceRepository>();

            builder.RegisterType<Repository.ContentManagement.ContentManagementServiceRepositoryImpl>().As<IContentManagementServiceRepository>();
            builder.RegisterType<com.benekeith.DivisionService.DivisionServiceClient>().As<com.benekeith.DivisionService.IDivisionService>();
            builder.RegisterType<com.benekeith.InvoiceService.InvoiceServiceClient>().As<com.benekeith.InvoiceService.IInvoiceService>();
            builder.RegisterType<com.benekeith.ListService.ListServcieClient>().As<com.benekeith.ListService.IListServcie>();
            builder.RegisterType<com.benekeith.MessagingService.MessagingServiceClient>().As<com.benekeith.MessagingService.IMessagingService>();
            builder.RegisterType<com.benekeith.OnlinePaymentService.OnlinePaymentServiceClient>().As<com.benekeith.OnlinePaymentService.IOnlinePaymentService>();
            builder.RegisterType<com.benekeith.OrderService.OrderServiceClient>().As<com.benekeith.OrderService.IOrderService>();
            builder.RegisterType<com.benekeith.ContentManagementService.ContentManagementServiceClient>().As<com.benekeith.ContentManagementService.IContentManagementService>();
            builder.RegisterType<com.benekeith.ReportService.ReportServiceClient>().As<com.benekeith.ReportService.IReportService>();

			builder.RegisterType<com.benekeith.ConfigurationService.ConfigurationServiceClient>().As<com.benekeith.ConfigurationService.IConfigurationService>();

			builder.RegisterGeneric(typeof(ModelExportLogicImpl<>)).As(typeof(IModelExportLogic<>));

            // Build the container.
            var container = builder.Build();

            // Create the depenedency resolver.
            var resolver = new AutofacWebApiDependencyResolver(container);
            return resolver;
        }
    }
}