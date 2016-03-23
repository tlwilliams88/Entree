
using KeithLink.Common.Core.AuditLog;
using KeithLink.Common.Core.Logging;

using KeithLink.Common.Impl.AuditLog;
using KeithLink.Common.Impl.Logging;

using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.Export;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.OnlinePayments.Payment;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.PowerMenu;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Interface.SingleSignOn;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.SpecialOrders;

using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.Configurations;
using KeithLink.Svc.Impl.Logic.ContentManagement;
using KeithLink.Svc.Impl.Logic.Export;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Impl.Logic.Invoices;
using KeithLink.Svc.Impl.Logic.Messaging;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.PowerMenu;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Logic.Reports;
using KeithLink.Svc.Impl.Logic.SingleSignOn;
using KeithLink.Svc.Impl.Logic.SiteCatalog;

using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.Brands;
using KeithLink.Svc.Impl.Repository.Cache;
using KeithLink.Svc.Impl.Repository.Configurations;
using KeithLink.Svc.Impl.Repository.ContentManagement;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Email;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.InternalCatalog;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.OnlinePayments;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Customer;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Invoice;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Payment;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.PowerMenu;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Profile.PasswordReset;
using KeithLink.Svc.Impl.Repository.SingleSignOn;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.Reports;
using KeithLink.Svc.Impl.Repository.SiteCatalog;

// this will be leaving soon!
//using KeithLink.Svc.WebApi.Repository.Profile;

using Autofac;
using Autofac.Integration.WebApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Features.Indexed;

namespace KeithLink.Svc.Impl.Repository.Autofac
{
    public static class DependencyMapFactory
    {
        public static ContainerBuilder GetWebApiContainer()
        {
            // Create the container builder.
            ContainerBuilder builder = new ContainerBuilder();

            // Register the Web API controllers.
            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());

            ///////////////////////////////////////////////////////////////////////////////
            // Repositories
            ///////////////////////////////////////////////////////////////////////////////

            // cache
            builder.RegisterType<CacheRepositoryImpl>().As<ICacheRepository>();
            builder.RegisterType<CacheRefreshRepositoryImpl>().As<ICacheRefreshRepository>();

            // cart
            builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
            builder.RegisterType<ShipDateRepositoryImpl>().As<IShipDateRepository>();

            // catalog
            builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();
            builder.Register(b => new BrandRepositoryImpl()).As<IBrandRepository>().InstancePerRequest();
            builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
            builder.Register(c => new ElasticSearchCatalogRepositoryImpl()).As<ICatalogRepository>().InstancePerRequest();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.Register(pi => new ProductImageRepositoryImpl()).As<IProductImageRepository>().InstancePerRequest();
            builder.RegisterType<ExternalCatalogRepositoryImpl>().As<IExternalCatalogRepository>();

            // customer
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();

            // division
            builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();

            // DSR
            builder.RegisterType<DsrRepositoryImpl>().As<IDsrRepository>();

            // lists
            builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();

            // marketing
            builder.RegisterType<ContentManagementExternalRepositoryImpl>().As<IContentManagementExternalRepository>();

            // messaging
            builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
            builder.RegisterType<UserPushNotificationDeviceRepositoryImpl>().As<IUserPushNotificationDeviceRepository>();
            builder.RegisterType<AmazonPushNotificationMessageProvider>().As<IPushNotificationMessageProvider>();
            builder.RegisterType<MessageTemplateRepositoryImpl>().As<IMessageTemplateRepository>();

            // order
            builder.RegisterType<ImagingRepositoryImpl>().As<IImagingRepository>();
            builder.RegisterType<ItemHistoryRepositoryImpl>().As<IItemHistoryRepository>();
            builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<IOrderSocketConnectionRepository>();
            builder.RegisterType<OrderUpdateRequestSocketRepositoryImpl>().As<IOrderUpdateSocketConnectionRepository>();
            builder.RegisterType<NoSpecialOrderRepositoryImpl>().As<ISpecialOrderRepository>();

            // other
            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();
            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
            builder.Register(l => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
            builder.RegisterType<ExportSettingRepositoryImpl>().As<IExportSettingRepository>();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            builder.RegisterType<ReportRepository>().As<IReportRepository>();

            // profile 
            builder.RegisterType<AvatarRepositoryImpl>().As<IAvatarRepository>();
            builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
            builder.RegisterType<NoDsrAliasRepositoryImpl>().As<IDsrAliasRepository>();
            //builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();  // this is also found in the DEMO preprocessor directive later
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
//            builder.RegisterType<MarketingPreferencesServiceRepositoryImpl>().As<IMarketingPreferencesServiceRepository>();
            builder.RegisterType<NoSettingsRepositoryImpl>().As<ISettingsRepository>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();

            ///////////////////////////////////////////////////////////////////////////////
            // Logic Classes
            ///////////////////////////////////////////////////////////////////////////////

            // cart
            builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
            builder.RegisterType<ShoppingCartLogicImpl>().As<IShoppingCartLogic>();

            // catalog
            builder.RegisterType<SiteCatalogLogicImpl>().As<Core.Interface.SiteCatalog.ICatalogLogic>();
            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();

            // division
            builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();

            // DSR
            builder.RegisterType<DsrLogic>().As<IDsrLogic>();

            // lists
            builder.RegisterType<InventoryValuationReportLogicImpl>().As<IInventoryValuationReportLogic>();

            //marketing 
            builder.RegisterType<ContentManagementLogicImpl>().As<IContentManagementLogic>();

            // messaging
            builder.RegisterType<MessagingLogicImpl>().As<IMessagingLogic>();
            builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();

            // order
            builder.RegisterType<ImagingLogicImpl>().As<IImagingLogic>();
            builder.RegisterType<ExportSettingLogicImpl>().As<IExportSettingLogic>();
            builder.RegisterType<OrderHistoryRequestLogicImpl>().As<IOrderHistoryRequestLogic>();
            builder.RegisterType<OrderLogicImpl>().As<IOrderLogic>();
            builder.RegisterType<OrderQueueLogicImpl>().As<IOrderQueueLogic>();

            // other
            builder.RegisterType<ImportLogicImpl>().As<IImportLogic>();
            builder.RegisterGeneric(typeof(ModelExportLogicImpl<>)).As(typeof(IModelExportLogic<>));
            builder.RegisterType<ReportLogic>().As<IReportLogic>();

            // profile
            builder.RegisterType<NoDsrAliasLogicImpl>().As<IDsrAliasLogic>();
            builder.RegisterType<NoSettingsLogicImpl>().As<ISettingsLogicImpl>();
            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();

            ///////////////////////////////////////////////////////////////////////////////
            // still moving
            ///////////////////////////////////////////////////////////////////////////////


#if DEMO
			builder.RegisterType<DemoExternalUserDomainRepositoryImpl>().As<ICustomerDomainRepository>();
			builder.RegisterType<DemoGenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#else
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#endif

            ///////////////////////////////////////////////////////////////////////////////
            // service repositories
            ///////////////////////////////////////////////////////////////////////////////
            //builder.RegisterType<Repository.Lists.ListServiceRepositoryImpl>().As<IListServiceRepository>();
            //builder.RegisterType<Repository.OnlinePayments.OnlinePaymentServiceRepositoryImpl>().As<IOnlinePaymentServiceRepository>();
            //builder.RegisterType<Repository.Orders.OrderServiceRepositoryImpl>().As<IOrderServiceRepository>();
            //builder.RegisterType<Repository.Invoices.InvoiceServiceRepositoryImpl>().As<IInvoiceServiceRepository>();

            //builder.RegisterType<com.benekeith.InvoiceService.InvoiceServiceClient>().As<com.benekeith.InvoiceService.IInvoiceService>();
            //builder.RegisterType<com.benekeith.ListService.ListServcieClient>().As<com.benekeith.ListService.IListServcie>();
            //builder.RegisterType<com.benekeith.OnlinePaymentService.OnlinePaymentServiceClient>().As<com.benekeith.OnlinePaymentService.IOnlinePaymentService>();
            //builder.RegisterType<com.benekeith.OrderService.OrderServiceClient>().As<com.benekeith.OrderService.IOrderService>();
            //builder.RegisterType<com.benekeith.ProfileService.ProfileServiceClient>().As<com.benekeith.ProfileService.IProfileService>();
            //builder.RegisterType<PasswordResetServiceImpl>().As<IPasswordResetService>();
            //builder.RegisterType<DsrAliasServiceImpl>().As<IDsrAliasService>();

            // removed
            //builder.RegisterType<Repository.Configurations.ExportSettingServiceRepositoryImpl>().As<IExportSettingServiceRepository>();
            //builder.RegisterType<Repository.Configurations.ExternalCatalogServiceRepositoryImpl>().As<IExternalCatalogServiceRepository>();

            // Build the container.
            return builder;
        }
        public static ContainerBuilder BuildQueueSvcContainer()
        {

            ContainerBuilder builder = new ContainerBuilder();

#if DEMO
				builder.RegisterType<DemoStagingRepositoryImpl>().As<IStagingRepository>();
				builder.RegisterType<DemoGenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#else
            builder.RegisterType<StagingRepositoryImpl>().As<IStagingRepository>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#endif


            builder.RegisterType<CatalogInternalRepositoryImpl>().As<ICatalogInternalRepository>();
            builder.RegisterType<CatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.ETL.ICatalogLogic>();

            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();
            builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
            builder.RegisterType<InternalBasketRepository>().As<IInternalBasketRepository>();
            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<CustomerLogicImpl>().As<ICustomerLogic>();
            builder.RegisterType<KeithLink.Svc.Impl.Logic.InternalSvc.InternalListLogic>().As<IInternalListLogic>();
            builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            //2nd builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();
            builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
            // 2nd builder.RegisterType<CustomerLogicImpl>().As <ICustomerLogic>();
            builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
            builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();

            builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
            builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
            builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
            builder.RegisterType<SiteCatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic>();
            builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            builder.RegisterType<InternalOrderHistoryLogic>().As<IInternalOrderHistoryLogic>();
            builder.RegisterType<InternalSpecialOrderLogic>().As<IInternalSpecialOrderLogic>();
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();
            builder.RegisterType<InternalInvoiceLogic>().As<IInternalInvoiceLogic>();
            builder.RegisterType<InvoiceRepositoryImpl>().As<IInvoiceRepository>();
            //builder.RegisterType<InternalContentManagementLogic>().As<IInternalContentManagementLogic>();
            //builder.RegisterType<ContentManagementItemRepositoryImpl>().As<IContentManagementItemRepository>();

            builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();
            builder.RegisterType<InvoiceItemRepositoryImpl>().As<IInvoiceItemRepository>();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
            builder.RegisterType<ListItemRepositoryImpl>().As<IListItemRepository>();

            builder.RegisterType<ListShareRepositoryImpl>().As<IListShareRepository>();

            //added 11-6
            builder.RegisterType<CustomerTopicRepositoryImpl>().As<ICustomerTopicRepository>();
            builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
            builder.RegisterType<UserPushNotificationDeviceRepositoryImpl>().As<IUserPushNotificationDeviceRepository>();

            builder.RegisterType<NotificationQueueConsumerImpl>().As<INotificationQueueConsumer>();
            builder.RegisterType<AmazonPushNotificationMessageProvider>().As<IPushNotificationMessageProvider>();

            builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();
            builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();
            builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();
            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
            builder.RegisterType<MessageTemplateRepositoryImpl>().As<IMessageTemplateRepository>();
            builder.RegisterType<InternalOrderLogicImpl>().As<IInternalOrderLogic>();
            builder.RegisterType<ExportSettingLogicImpl>().As<IExportSettingLogic>();

            // keyed types - notification handlers
            builder.RegisterType<OrderConfirmationNotificationHandlerImpl>()
                .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.OrderConfirmation);
            builder.RegisterType<InvoiceNotificationHandlerImpl>()
                .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.InvoiceAttention);
            builder.RegisterType<EtaNotificationHandlerImpl>()
                .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.Eta);
            builder.RegisterType<PaymentConfirmationNotificationHandlerImpl>()
                .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.PaymentConfirmation);
            builder.RegisterType<HasNewsNotificationHandlerImpl>()
                .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.HasNews);
            builder.Register<Func<Svc.Core.Enumerations.Messaging.NotificationType, INotificationHandler>>(
                c =>
                {
                    var handlers = c.Resolve<IIndex<Svc.Core.Enumerations.Messaging.NotificationType, INotificationHandler>>();
                    return request => handlers[request];
                });

            // keyed types - message providers
            builder.RegisterType<WebMessageProvider>()
                .Keyed<IMessageProvider>(Svc.Core.Enumerations.Messaging.Channel.Web);
            builder.RegisterType<EmailMessageProvider>()
                .Keyed<IMessageProvider>(Svc.Core.Enumerations.Messaging.Channel.Email);
            builder.RegisterType<AmazonPushNotificationMessageProvider>()
                .Keyed<IMessageProvider>(Svc.Core.Enumerations.Messaging.Channel.MobilePush);
            builder.Register<Func<Svc.Core.Enumerations.Messaging.Channel, IMessageProvider>>(
                c =>
                {
                    var handlers = c.Resolve<IIndex<Svc.Core.Enumerations.Messaging.Channel, IMessageProvider>>();
                    return request => handlers[request];
                });

            // no implementation (will throw notimplementedexception if called)
            builder.RegisterType<NoOrderServiceRepositoryImpl>().As<IOrderServiceRepository>();
            builder.RegisterType<NoListServiceRepositoryImpl>().As<IListServiceRepository>();
            //builder.RegisterType<NoDsrServiceRepository>().As<IDsrServiceRepository>();

            builder.RegisterType<TermRepositoryImpl>().As<ITermRepository>();
            builder.RegisterType<NoInvoiceServiceRepositoryImpl>().As<IInvoiceServiceRepository>();

            // customer bank - JA - 11/13<
            builder.RegisterType<KPayDBContext>().As<IKPayDBContext>();
            builder.RegisterType<CustomerBankRepositoryImpl>().As<ICustomerBankRepository>();
            builder.RegisterType<NoOnlinePaymentServiceRepository>().As<IOnlinePaymentServiceRepository>();
            builder.RegisterType<KPayInvoiceRepositoryImpl>().As<IKPayInvoiceRepository>();
            builder.RegisterType<KPayPaymentTransactionRepositoryImpl>().As<IKPayPaymentTransactionRepository>();

            builder.RegisterType<InternalOnlinePaymentLogicImpl>().As<IOnlinePaymentsLogic>();

            builder.RegisterType<ExportSettingRepositoryImpl>().As<IExportSettingRepository>();
            builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();
            builder.RegisterType<ExternalCatalogRepositoryImpl>().As<IExternalCatalogRepository>();

            builder.RegisterType<ReportRepository>().As<IReportRepository>();

            // order conversion - JA - 1/8/15
            builder.RegisterType<OrderConversionLogicImpl>().As<IOrderConversionLogic>();

            builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
            builder.RegisterType<CacheRefreshRepositoryImpl>().As<ICacheRefreshRepository>();

            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();

            // dsr repository
            builder.RegisterType<DsrRepositoryImpl>().As<IDsrRepository>();
            builder.RegisterType<DsrLogic>().As<IDsrLogic>();

            builder.RegisterType<NoPasswordResetServiceRepositoryImpl>().As<IPasswordResetService>();
            builder.RegisterType<NoSettingsLogicImpl>().As<ISettingsLogicImpl>();

            //profile
            builder.RegisterType<SettingsLogicImpl>().As<ISettingsLogicImpl>();
            builder.RegisterType<SettingsRepositoryImpl>().As<ISettingsRepository>();

            // messaging
            builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
            builder.RegisterType<UserPushNotificationDeviceRepositoryImpl>().As<IUserPushNotificationDeviceRepository>();
            builder.RegisterType<AmazonPushNotificationMessageProvider>().As<IPushNotificationMessageProvider>();
            builder.RegisterType<MessagingLogicImpl>().As<IMessagingLogic>();
            builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();

            return builder;
        }
        public static ContainerBuilder BuildAccessServiceContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();

#if DEMO
				builder.RegisterType<DemoStagingRepositoryImpl>().As<IStagingRepository>();
				builder.RegisterType<DemoGenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#else
            builder.RegisterType<StagingRepositoryImpl>().As<IStagingRepository>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#endif

            // PowerMenu
            builder.RegisterType<PowerMenuRepositoryImpl>().As<IPowerMenuRepository>();
            builder.RegisterType<PowerMenuLogicImpl>().As<IPowerMenuLogic>();


            // Service
            builder.RegisterType<AccessRequestLogicImpl>().As<IAccessRequestLogic>();
            builder.RegisterType<KbitRequestLogicImpl>().As<IKbitRequestLogic>();
            builder.RegisterType<KbitRepositoryImpl>().As<IKbitRepository>();


            return builder;
        }
        public static ContainerBuilder BuildOrderServiceContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();

            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();

            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();

            builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();
            builder.RegisterType<OrderQueueLogicImpl>().As<IOrderQueueLogic>();
            builder.RegisterType<OrderHistoryRequestLogicImpl>().As<IOrderHistoryRequestLogic>();
            builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();
            builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<IOrderSocketConnectionRepository>();
            builder.RegisterType<OrderUpdateRequestSocketRepositoryImpl>().As<IOrderUpdateSocketConnectionRepository>();
            builder.RegisterType<NoOrderConversionLogicImpl>().As<IOrderConversionLogic>();
            builder.RegisterType<SpecialOrderRepositoryImpl>().As<ISpecialOrderRepository>();
            builder.RegisterType<SpecialOrderDBContext>().As<ISpecialOrderDBContext>();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            return builder;
        }
    }
}
