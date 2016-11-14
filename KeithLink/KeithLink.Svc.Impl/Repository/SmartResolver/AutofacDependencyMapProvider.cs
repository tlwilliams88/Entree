using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Core.Interfaces.Settings;

using KeithLink.Common.Impl.Logic.Settings;

using KeithLink.Common.Impl.Repository.Logging;
using KeithLink.Common.Impl.Repository.Settings;

using KeithLink.Svc.Core.Enumerations.Messaging;

using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Interface.Customers;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.ETL.ElasticSearch;
using KeithLink.Svc.Core.Interface.Export;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.OnlinePayments.Log;
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

using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.Configurations;
using KeithLink.Svc.Impl.Logic.ContentManagement;
using KeithLink.Svc.Impl.Logic.ETL;
using KeithLink.Svc.Impl.Logic.Export;
using KeithLink.Svc.Impl.Logic.Invoices;
using KeithLink.Svc.Impl.Logic.Lists;
using KeithLink.Svc.Impl.Logic.Messaging;
using KeithLink.Svc.Impl.Logic.OnlinePayments;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.PowerMenu;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Logic.Profile.PasswordRequest;
using KeithLink.Svc.Impl.Logic.Reports;
using KeithLink.Svc.Impl.Logic.SingleSignOn;
using KeithLink.Svc.Impl.Logic.SiteCatalog;

using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.Brands;
using KeithLink.Svc.Impl.Repository.Cache;
using KeithLink.Svc.Impl.Repository.Configurations;
using KeithLink.Svc.Impl.Repository.ContentManagement;
using KeithLink.Svc.Impl.Repository.Customers;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Email;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.InternalCatalog;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Customer;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Invoice;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Log;
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

using Autofac;
using Autofac.Features.Indexed;

using System;
using KeithLink.Svc.Core.Enumerations.Dependencies;
using KeithLink.Svc.Impl.Logic.SiteCatalog.Images.External;
using KeithLink.Svc.Core.Interface.ApplicationHealth;
using KeithLink.Svc.Impl.Logic.ApplicationHealth;

namespace KeithLink.Svc.Impl.Repository.SmartResolver
{
    internal static class AutofacDependencyMapProvider
    {
        internal static void BuildBaselineDependencies(ContainerBuilder builder, DependencyInstanceType type = DependencyInstanceType.InstancePerLifetimeScope) {
            ///////////////////////////////////////////////////////////////////////////////
            // Repositories
            ///////////////////////////////////////////////////////////////////////////////

            // cart
            builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
            builder.RegisterType<ShipDateRepositoryImpl>().As<IShipDateRepository>();

            // catalog
            builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();
            builder.RegisterType<BrandRepositoryImpl>().As<IBrandRepository>().InstancePerRequest();
            builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>().InstancePerRequest();
            builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>().InstancePerRequest();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<ExternalCatalogRepositoryImpl>().As<IExternalCatalogRepository>();

            // customer
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<InternalUserAccessRepository>().As<IInternalUserAccessRepository>();

            // division
            builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();

            // DSR
            builder.RegisterType<DsrRepositoryImpl>().As<IDsrRepository>();

            // invoices
            builder.RegisterType<CustomerBankRepositoryImpl>().As<ICustomerBankRepository>();
            builder.RegisterType<KPayInvoiceRepositoryImpl>().As<IKPayInvoiceRepository>();
            builder.RegisterType<KPayLogRepositoryImpl>().As<IKPayLogRepository>();
            builder.RegisterType<KPayPaymentTransactionRepositoryImpl>().As<IKPayPaymentTransactionRepository>();
            builder.RegisterType<TermRepositoryImpl>().As<ITermRepository>();
            builder.RegisterType<KPayDBContext>().As<IKPayDBContext>();

            // lists
            builder.RegisterType<ListItemRepositoryImpl>().As<IListItemRepository>();
            builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
            builder.RegisterType<ListShareRepositoryImpl>().As<IListShareRepository>();
            builder.RegisterType<CustomInventoryItemsRepositoryImpl>().As<ICustomInventoryItemsRepository>();

            // marketing
            builder.RegisterType<ContentManagementExternalRepositoryImpl>().As<IContentManagementExternalRepository>();

            // messaging
            builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
            builder.RegisterType<UserPushNotificationDeviceRepositoryImpl>().As<IUserPushNotificationDeviceRepository>();
            builder.RegisterType<AmazonPushNotificationMessageProvider>().As<IPushNotificationMessageProvider>();
            builder.RegisterType<MessageTemplateRepositoryImpl>().As<IMessageTemplateRepository>();
            builder.RegisterType<PushMessageConsumerImpl>().As<IPushMessageConsumer>();

            // order
            builder.RegisterType<ImagingRepositoryImpl>().As<IImagingRepository>();
            builder.RegisterType<ItemHistoryRepositoryImpl>().As<IItemHistoryRepository>();
            builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();
            builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<IOrderSocketConnectionRepository>();
            builder.RegisterType<OrderUpdateRequestSocketRepositoryImpl>().As<IOrderUpdateSocketConnectionRepository>();
            builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();
            builder.RegisterType<SpecialOrderLogicImpl>().As<ISpecialOrderLogic>();
            builder.RegisterType<SpecialOrderRepositoryImpl>().As<ISpecialOrderRepository>();
            builder.RegisterType<SpecialOrderDBContext>().As<ISpecialOrderDBContext>();
            builder.RegisterType<OrderHistoryWriterImpl>().As<IOrderHistoryWriter>();

            // other
            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();
            builder.RegisterType<DBAppSettingsRepositoryImpl>().As<IDBAppSettingsRepository>();
            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
            builder.Register(l => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
            builder.RegisterType<ExportSettingRepositoryImpl>().As<IExportSettingRepository>();
            builder.RegisterType<PowerMenuRepositoryImpl>().As<IPowerMenuRepository>();
            builder.RegisterType<ReportRepository>().As<IReportRepository>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();
            builder.RegisterType<ApplicationHealthLogicImpl>().As<IApplicationHealthLogic>();

            // profile 
            builder.RegisterType<AvatarRepositoryImpl>().As<IAvatarRepository>();
            builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
            builder.RegisterType<DsrAliasRepositoryImpl>().As<IDsrAliasRepository>();
            builder.RegisterType<MarketingPreferencesRepositoryImpl>().As<IMarketingPreferencesRepository>();
            builder.RegisterType<PasswordResetRequestRepositoryImpl>().As<IPasswordResetRequestRepository>();
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            builder.RegisterType<SettingsRepositoryImpl>().As<ISettingsRepository>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();

            ///////////////////////////////////////////////////////////////////////////////
            // Logic Classes
            ///////////////////////////////////////////////////////////////////////////////

            // cart
            builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
            builder.RegisterType<ShoppingCartLogicImpl>().As<IShoppingCartLogic>();

            // catalog
            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();
            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            builder.RegisterType<SiteCatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic>();
            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
            builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
            builder.RegisterType<UnfiImageProcessing>().As<IExternalImageProcessorUnfi>();

            // customer
            builder.RegisterType<CustomerLogicImpl>().As<ICustomerLogic>();

            // division
            builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();

            // DSR
            builder.RegisterType<DsrLogic>().As<IDsrLogic>();

            // invoices
            builder.RegisterType<ImagingLogicImpl>().As<IImagingLogic>();
            builder.RegisterType<OnlinePaymentLogicImpl>().As<IOnlinePaymentsLogic>();
            builder.RegisterType<TermLogicImpl>().As<ITermLogic>();

            // lists
            builder.RegisterType<FavoriteLogicImpl>().As<IFavoriteLogic>();
            builder.RegisterType<HistoryLogic>().As<IHistoryLogic>();
            builder.RegisterType<ListLogicImpl>().As<IListLogic>();
            builder.RegisterType<NoteLogicImpl>().As<INoteLogic>();

            //marketing 
            builder.RegisterType<ContentManagementLogicImpl>().As<IContentManagementLogic>();

            // messaging
            builder.RegisterType<MessagingLogicImpl>().As<IMessagingLogic>();
            builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();
            builder.RegisterType<NotificationQueueConsumerImpl>().As<INotificationQueueConsumer>();
            builder.RegisterType<OrderConfirmationNotificationHandlerImpl>().Keyed<INotificationHandler>(NotificationType.OrderConfirmation);
            builder.RegisterType<InvoiceNotificationHandlerImpl>().Keyed<INotificationHandler>(NotificationType.InvoiceAttention);
            builder.RegisterType<EtaNotificationHandlerImpl>().Keyed<INotificationHandler>(NotificationType.Eta);
            builder.RegisterType<PaymentConfirmationNotificationHandlerImpl>().Keyed<INotificationHandler>(NotificationType.PaymentConfirmation);
            builder.RegisterType<HasNewsNotificationHandlerImpl>().Keyed<INotificationHandler>(NotificationType.HasNews);
            builder.Register<Func<NotificationType, INotificationHandler>>(
                c => {
                    var handlers = c.Resolve<IIndex<NotificationType, INotificationHandler>>();
                    return request => handlers[request];
                });
            // keyed types - message providers
            builder.RegisterType<WebMessageProvider>().Keyed<IMessageProvider>(Channel.Web);
            builder.RegisterType<EmailMessageProvider>().Keyed<IMessageProvider>(Channel.Email);
            builder.RegisterType<PushMessagePublisherImpl>().Keyed<IMessageProvider>(Channel.MobilePush);
            builder.Register<Func<Channel, IMessageProvider>>(
                c => {
                    var handlers = c.Resolve<IIndex<Channel, IMessageProvider>>();
                    return request => handlers[request];
                });

            // order
            builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            builder.RegisterType<ExportSettingLogicImpl>().As<IExportSettingLogic>();
            builder.RegisterType<OrderConversionLogicImpl>().As<IOrderConversionLogic>();
            builder.RegisterType<OrderHistoryRequestLogicImpl>().As<IOrderHistoryRequestLogic>();
            builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            builder.RegisterType<OrderLogicImpl>().As<IOrderLogic>();
            builder.RegisterType<OrderQueueLogicImpl>().As<IOrderQueueLogic>();
            builder.RegisterType<UserActiveCartLogicImpl>().As<IUserActiveCartLogic>();

            // other
            builder.RegisterType<AppSettingLogicImpl>().As<IAppSettingLogic>();
            builder.RegisterType<ImportLogicImpl>().As<IImportLogic>();
            builder.RegisterGeneric(typeof(ModelExportLogicImpl<>)).As(typeof(IModelExportLogic<>));
            builder.RegisterType<PowerMenuLogicImpl>().As<IPowerMenuLogic>();

            // profile
            builder.RegisterType<DsrAliasLogicImpl>().As<IDsrAliasLogic>();
            builder.RegisterType<MarketingPreferencesLogicImpl>().As<IMarketingPreferencesLogic>();
            builder.RegisterType<PasswordResetLogicImpl>().As<IPasswordResetLogic>();
            builder.RegisterType<SettingsLogicImpl>().As<ISettingsLogic>();
            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();

            // reports
            builder.RegisterType<InventoryValuationReportLogicImpl>().As<IInventoryValuationReportLogic>();
            builder.RegisterType<ReportLogic>().As<IReportLogic>();

#if DEMO
			builder.RegisterType<DemoExternalUserDomainRepositoryImpl>().As<ICustomerDomainRepository>();
			builder.RegisterType<DemoGenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#else
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
            builder.RegisterType<GenericSubscriptionQueueRepositoryImpl>().As<IGenericSubsriptionQueueRepository>();
#endif

            AddDatabaseDependencies(builder, type);
        }

        private static void AddDatabaseDependencies(ContainerBuilder builder, DependencyInstanceType type = DependencyInstanceType.None) {
            switch (type) {
                case DependencyInstanceType.InstancePerLifetimeScope:
                    builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
                    break;
                case DependencyInstanceType.InstancePerRequest:
                    builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
                    break;
                case DependencyInstanceType.SingleInstance:
                    builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().SingleInstance();
                    break;
                case DependencyInstanceType.InstancePerDependency:
                    builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
                    break;
                default:
                    builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
                    break;
            }
        }

        internal static void AddOtherInternalServiceDependencies(ContainerBuilder builder)
        {
            // cache
            builder.RegisterType<CacheRepositoryImpl>().As<ICacheRepository>();
            builder.RegisterType<CacheRefreshRepositoryImpl>().As<ICacheRefreshRepository>();

            // ETL logic
            builder.RegisterType<CatalogLogicImpl>().As<Core.Interface.ETL.ICatalogLogic>();
            builder.RegisterType<CategoriesImportLogicImpl>().As<ICategoriesImport>();
            builder.RegisterType<CustomerLogicImpl>().As<ICustomerLogic>();
            builder.RegisterType<HouseBrandsImportLogicImpl>().As<IHouseBrandsImport>();
            builder.RegisterType<ItemImportLogicImpl>().As<IItemImport>();
            builder.RegisterType<ListImportLogicImpl>().As<IListsImportLogic>();

            // ETL repositories
            builder.RegisterType<CatalogInternalRepositoryImpl>().As<ICatalogInternalRepository>();
            builder.RegisterType<StagingRepositoryImpl>().As<IStagingRepository>();
        }

        internal static void AddOtherWebApiDependencies(ContainerBuilder builder)
        {
            // cache
            builder.RegisterType<CacheRepositoryImpl>().As<ICacheRepository>();
            builder.RegisterType<CacheRefreshRepositoryImpl>().As<ICacheRefreshRepository>();
        }

        internal static void AddOtherMonitorServiceDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
        }

        internal static void AddOtherQueueServiceDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
        }

        internal static void AddOtherCatalogServiceDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
        }

        internal static void AddOtherOrderServiceDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
        }

        internal static void AddOtherAccessServiceDependencies(ContainerBuilder builder)
        {
            // Service
            builder.RegisterType<AccessRequestLogicImpl>().As<IAccessRequestLogic>();
            builder.RegisterType<KbitRequestLogicImpl>().As<IKbitRequestLogic>();
            builder.RegisterType<KbitRepositoryImpl>().As<IKbitRepository>();
        }
    }
}
