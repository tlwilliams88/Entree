using KeithLink.Common.Core.AuditLog;
using KeithLink.Common.Core.Logging;

using KeithLink.Common.Impl.AuditLog;
using KeithLink.Common.Impl.Logging;

using KeithLink.Svc.Core.Enumerations.Messaging;

using KeithLink.Svc.Core.Interface;
using KeithLink.Svc.Core.Interface.Brand;
using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.ContentManagement;
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

namespace KeithLink.Svc.Impl.Repository.SmartResolver
{
    internal static class AutofacDependencyMapProvider
    {
        internal static void BuildBaselineDependencies(ContainerBuilder builder) {
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
            builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();
            builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<IOrderSocketConnectionRepository>();
            builder.RegisterType<OrderUpdateRequestSocketRepositoryImpl>().As<IOrderUpdateSocketConnectionRepository>();
            builder.RegisterType<SpecialOrderRepositoryImpl>().As<ISpecialOrderRepository>();
            builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();
            builder.RegisterType<SpecialOrderRepositoryImpl>().As<ISpecialOrderRepository>();
            builder.RegisterType<SpecialOrderDBContext>().As<ISpecialOrderDBContext>();

            // other
            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();
            builder.Register(l => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
            builder.RegisterType<ExportSettingRepositoryImpl>().As<IExportSettingRepository>();
            builder.RegisterType<PowerMenuRepositoryImpl>().As<IPowerMenuRepository>();
            builder.RegisterType<ReportRepository>().As<IReportRepository>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();

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
            builder.RegisterType<SiteCatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic>();
            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();

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
            builder.RegisterType<AmazonPushNotificationMessageProvider>().Keyed<IMessageProvider>(Channel.MobilePush);
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
#endif
        }

        internal static void AddDatabaseDependencies(ContainerBuilder builder, DependencyInstanceType type = DependencyInstanceType.None) {
            if(type == DependencyInstanceType.InstancePerLifetimeScope) {
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            }
            else if (type == DependencyInstanceType.SingleInstance)
            {
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().SingleInstance();
            }
            else if (type == DependencyInstanceType.InstancePerRequest)
            {
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            }
            else {
                builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
            }
        }

        internal static void AddOtherInternalServiceDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<CatalogInternalRepositoryImpl>().As<ICatalogInternalRepository>();
            builder.RegisterType<CatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.ETL.ICatalogLogic>();

            //builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();
            //builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
            //builder.RegisterType<InternalBasketRepository>().As<IInternalBasketRepository>();
            //builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
            //builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            //builder.RegisterType<CustomerLogicImpl>().As<ICustomerLogic>();
            //builder.RegisterType<ListLogicImpl>().As<IListLogic>();
            //builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
            //builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            ////2nd builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            //builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
            //builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            //builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            //builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            //builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            //builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();
            //builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
            //builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
            //builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();

            builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
            builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
            builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
            builder.RegisterType<SiteCatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic>();
            builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();

            builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
            builder.RegisterType<ListItemRepositoryImpl>().As<IListItemRepository>();

            builder.RegisterType<ListShareRepositoryImpl>().As<IListShareRepository>();

            //added 11-6
            builder.RegisterType<CustomerTopicRepositoryImpl>().As<ICustomerTopicRepository>();
            builder.RegisterType<MessagingLogicImpl>().As<IMessagingLogic>();
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
                c => {
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
                c => {
                    var handlers = c.Resolve<IIndex<Svc.Core.Enumerations.Messaging.Channel, IMessageProvider>>();
                    return request => handlers[request];
                });

            builder.RegisterType<TermRepositoryImpl>().As<ITermRepository>();
            builder.RegisterType<TermLogicImpl>().As<ITermLogic>();

            // customer bank - JA - 11/13<
            // Local Dependencies?
            //builder.RegisterType<OnlinePaymentService>();
            builder.RegisterType<KPayDBContext>().As<IKPayDBContext>();
            builder.RegisterType<CustomerBankRepositoryImpl>().As<ICustomerBankRepository>();
            builder.RegisterType<KPayInvoiceRepositoryImpl>().As<IKPayInvoiceRepository>();
            builder.RegisterType<KPayLogRepositoryImpl>().As<IKPayLogRepository>();
            builder.RegisterType<KPayPaymentTransactionRepositoryImpl>().As<IKPayPaymentTransactionRepository>();

            builder.RegisterType<ExportSettingRepositoryImpl>().As<IExportSettingRepository>();
            builder.RegisterType<ExportSettingLogicImpl>().As<IExportSettingLogic>();
            builder.RegisterType<ExternalCatalogRepositoryImpl>().As<IExternalCatalogRepository>();
            builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();

            builder.RegisterType<ReportRepository>().As<IReportRepository>();

            // order conversion - JA - 1/8/15
            builder.RegisterType<OrderConversionLogicImpl>().As<IOrderConversionLogic>();

            builder.RegisterType<CacheRepositoryImpl>().As<ICacheRepository>();
            builder.RegisterType<CacheRefreshRepositoryImpl>().As<ICacheRefreshRepository>();


            // dsr repository
            builder.RegisterType<DsrRepositoryImpl>().As<IDsrRepository>();
            builder.RegisterType<DsrLogic>().As<IDsrLogic>();


            // PowerMenu
            builder.RegisterType<PowerMenuRepositoryImpl>().As<IPowerMenuRepository>();
            builder.RegisterType<PowerMenuLogicImpl>().As<IPowerMenuLogic>();

            //Password Reset
            builder.RegisterType<PasswordResetLogicImpl>().As<IPasswordResetLogic>();
            builder.RegisterType<PasswordResetRequestRepositoryImpl>().As<IPasswordResetRequestRepository>();

            // DSR Alias
            builder.RegisterType<DsrAliasRepositoryImpl>().As<IDsrAliasRepository>();
            builder.RegisterType<DsrAliasLogicImpl>().As<IDsrAliasLogic>();

            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();

            builder.RegisterType<MarketingPreferencesRepositoryImpl>().As<IMarketingPreferencesRepository>();
            builder.RegisterType<MarketingPreferencesLogicImpl>().As<IMarketingPreferencesLogic>();

            // ElasticSearch ETL 
            builder.RegisterType<ItemImportLogicImpl>().As<IItemImport>();
            builder.RegisterType<CategoriesImportLogicImpl>().As<ICategoriesImport>();
            builder.RegisterType<HouseBrandsImportLogicImpl>().As<IHouseBrandsImport>();

            // List ETL
            builder.RegisterType<ListImportLogicImpl>().As<IListsImportLogic>();

            // Item History
            builder.RegisterType<ItemHistoryRepositoryImpl>().As<IItemHistoryRepository>();

            // Profile Settings
            builder.RegisterType<SettingsRepositoryImpl>().As<ISettingsRepository>();
            builder.RegisterType<SettingsLogicImpl>().As<ISettingsLogic>();

            // messaging
            builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
            builder.RegisterType<UserPushNotificationDeviceRepositoryImpl>().As<IUserPushNotificationDeviceRepository>();
            builder.RegisterType<AmazonPushNotificationMessageProvider>().As<IPushNotificationMessageProvider>();
            builder.RegisterType<MessagingLogicImpl>().As<IMessagingLogic>();
            builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();

            // lists
            builder.RegisterType<FavoriteLogicImpl>().As<IFavoriteLogic>();
            builder.RegisterType<HistoryLogic>().As<IHistoryLogic>();
            builder.RegisterType<NoteLogicImpl>().As<INoteLogic>();

            builder.RegisterType<OnlinePaymentLogicImpl>().As<IOnlinePaymentsLogic>();

        }

        internal static void AddOtherWebApiDependencies(ContainerBuilder builder)
        {
            ///////////////////////////////////////////////////////////////////////////////
            // Repositories
            ///////////////////////////////////////////////////////////////////////////////

            // cache
            builder.RegisterType<CacheRepositoryImpl>().As<ICacheRepository>();
            builder.RegisterType<CacheRefreshRepositoryImpl>().As<ICacheRefreshRepository>();

            AddDatabaseDependencies(builder, DependencyInstanceType.InstancePerLifetimeScope);
        }

        internal static void AddOtherQueueServiceDependencies(ContainerBuilder builder)
        {
            AddDatabaseDependencies(builder, DependencyInstanceType.InstancePerLifetimeScope);
            //builder.RegisterType<CatalogInternalRepositoryImpl>().As<ICatalogInternalRepository>();
            //builder.RegisterType<CatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.ETL.ICatalogLogic>();

            builder.RegisterType<InternalBasketRepository>().As<IInternalBasketRepository>();
            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<CustomerLogicImpl>().As<ICustomerLogic>();
            builder.RegisterType<ListLogicImpl>().As<IListLogic>();
            builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();
            builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
            builder.RegisterType<CustomerLogicImpl>().As<ICustomerLogic>();
            builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
            builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();

            builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
            builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
            builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
            builder.RegisterType<SiteCatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic>();
            builder.RegisterType<OrderQueueLogicImpl>().As<IOrderQueueLogic>();
            builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            builder.RegisterType<SpecialOrderLogicImpl>().As<ISpecialOrderLogic>();
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();
            builder.RegisterType<ContentManagementLogicImpl>().As<IContentManagementLogic>();
            //builder.RegisterType<ContentManagementItemRepositoryImpl>().As<IContentManagementItemRepository>();
            builder.RegisterType<OrderLogicImpl>().As<IOrderLogic>();
            builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<IOrderSocketConnectionRepository>();
            builder.RegisterType<OrderUpdateRequestSocketRepositoryImpl>().As<IOrderUpdateSocketConnectionRepository>();
            //builder.RegisterType<SpecialOrderRepositoryImpl>().As<ISpecialOrderRepository>();
            //builder.RegisterType<SpecialOrderDBContext>().As<ISpecialOrderDBContext>();

            builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
        }

        internal static void AddOtherOrderServiceDependencies(ContainerBuilder builder)
        {
            //builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            //builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            //builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();
            //builder.RegisterType<OrderQueueLogicImpl>().As<IOrderQueueLogic>();
            //builder.RegisterType<OrderHistoryRequestLogicImpl>().As<IOrderHistoryRequestLogic>();
            //builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            //builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();
            //builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<IOrderSocketConnectionRepository>();
            //builder.RegisterType<OrderUpdateRequestSocketRepositoryImpl>().As<IOrderUpdateSocketConnectionRepository>();
            //builder.RegisterType<OrderConversionLogicImpl>().As<IOrderConversionLogic>();
            //builder.RegisterType<SpecialOrderRepositoryImpl>().As<ISpecialOrderRepository>();
            //builder.RegisterType<SpecialOrderDBContext>().As<ISpecialOrderDBContext>();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
        }

        internal static void AddOtherAccessServiceDependencies(ContainerBuilder builder)
        {
            // Service
            builder.RegisterType<AccessRequestLogicImpl>().As<IAccessRequestLogic>();
            builder.RegisterType<KbitRequestLogicImpl>().As<IKbitRequestLogic>();
            builder.RegisterType<KbitRepositoryImpl>().As<IKbitRepository>();
        }

        internal static void AddStagingDependency(ContainerBuilder builder)
        {
#if DEMO
				builder.RegisterType<DemoStagingRepositoryImpl>().As<IStagingRepository>();
				builder.RegisterType<DemoGenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#else
            builder.RegisterType<StagingRepositoryImpl>().As<IStagingRepository>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#endif
        }
    }
}
