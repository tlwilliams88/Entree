using KeithLink.Common.Core.Interfaces.Logging;
using KeithLink.Common.Core.Interfaces.Settings;

using KeithLink.Common.Impl.Logic.Settings;

using KeithLink.Common.Impl.Repository.Logging;
using KeithLink.Common.Impl.Repository.Settings;

using Entree.Core.Enumerations.Messaging;

using Entree.Core.Interface;
using Entree.Core.Interface.Brand;
using Entree.Core.Interface.Cache;
using Entree.Core.Interface.Cart;
using Entree.Core.Interface.Common;
using Entree.Core.Interface.Configurations;
using Entree.Core.Interface.ContentManagement;
using Entree.Core.Interface.Customers;
using Entree.Core.Interface.Email;
using Entree.Core.Interface.ETL;
using Entree.Core.Interface.ETL.ElasticSearch;
using Entree.Core.Interface.Export;
using Entree.Core.Interface.Invoices;
using Entree.Core.Interface.InternalCatalog;
using Entree.Core.Interface.Lists;
using Entree.Core.Interface.Marketing;
using Entree.Core.Interface.Messaging;
using Entree.Core.Interface.OnlinePayments;
using Entree.Core.Interface.OnlinePayments.Customer;
using Entree.Core.Interface.OnlinePayments.Invoice;
using Entree.Core.Interface.OnlinePayments.Log;
using Entree.Core.Interface.OnlinePayments.Payment;
using Entree.Core.Interface.Orders;
using Entree.Core.Interface.Orders.Confirmations;
using Entree.Core.Interface.Orders.History;
using Entree.Core.Interface.PowerMenu;
using Entree.Core.Interface.Profile;
using Entree.Core.Interface.Profile.PasswordReset;
using Entree.Core.Interface.Reports;
using Entree.Core.Interface.SingleSignOn;
using Entree.Core.Interface.SiteCatalog;
using Entree.Core.Interface.SpecialOrders;
using Entree.Core.Interface.UserFeedback;

using KeithLink.Svc.Impl.Service.ShoppingCart;
using KeithLink.Svc.Impl.Service;

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
using KeithLink.Svc.Impl.Logic.UserFeedback;

using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.Brands;
using Entree.Core.Cache.Repositories;
using KeithLink.Svc.Impl.Repository.Configurations;
using KeithLink.Svc.Impl.Repository.ContentManagement;
using KeithLink.Svc.Impl.Repository.Customers;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Email;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.InternalCatalog;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.Marketing;
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
using KeithLink.Svc.Impl.Repository.UserFeedback;

using Autofac;
using Autofac.Features.Indexed;

using System;
using Entree.Core.Enumerations.Dependencies;
using KeithLink.Svc.Impl.Logic.SiteCatalog.Images.External;
using Entree.Core.Interface.ApplicationHealth;
using Entree.Core.Interface.DataConnection;
using Entree.Core.Interface.Import;
using KeithLink.Svc.Impl.Logic.ApplicationHealth;
using KeithLink.Svc.Impl.Repository.Templates;
using Entree.Core.Interface.Templates;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Logic.Cache;
using KeithLink.Svc.Impl.Repository.DataConnection;
using KeithLink.Svc.Impl.Repository.Documents;
using KeithLink.Svc.Impl.Service.Invoices;
using KeithLink.Svc.Impl.Service.SiteCatalog;
using KeithLink.Svc.Impl.Service.List;
using Entree.Core.Interface.Documents;

namespace KeithLink.Svc.Impl.Repository.SmartResolver
{
    internal static class AutofacDependencyMapProvider
    {
        internal static void BuildBaselineDependencies(ContainerBuilder builder, DependencyInstanceType type = DependencyInstanceType.InstancePerLifetimeScope) {
            ///////////////////////////////////////////////////////////////////////////////
            // Repositories
            ///////////////////////////////////////////////////////////////////////////////
            
            // Dapper connection
            builder.Register(ctx => new DapperDatabaseConnection(Configuration.BEKDBConnectionString))
                   .As<IDapperDatabaseConnection>()
                   .InstancePerRequest();

            // Azure connection
            builder.Register(ctx => new AzureContainerConnection(Configuration.AzureConnectionString))
                   .As<IAzureContainerConnection>()
                   .InstancePerRequest();
            builder.RegisterType<AzureContainerRepositoryImpl>().As<IAzureContainerRepository>();


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

            // catalog campaign
            builder.RegisterType<CatalogCampaignHeaderRepositoryImpl>().As<ICatalogCampaignHeaderRepository>();
            builder.RegisterType<CatalogCampaignItemsRepositoryImpl>().As<ICatalogCampaignItemRepository>();

            // customer
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<InternalUserAccessRepository>().As<IInternalUserAccessRepository>();
            builder.RegisterType<RecommendedItemsRepositoryImpl>().As<IRecommendedItemsRepository>();
            builder.RegisterType<GrowthAndRecoveriesRepositoryImpl>().As<IGrowthAndRecoveriesRepository>();

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
            builder.RegisterType<FavoriteListHeaderRepositoryImpl>().As<IFavoriteListHeadersRepository>();
            builder.RegisterType<FavoriteListDetailRepositoryImpl>().As<IFavoriteListDetailsRepository>();
            builder.RegisterType<NotesHeadersRepositoryImpl>().As<INotesHeadersListRepository>();
            builder.RegisterType<NotesDetailsRepositoryImpl>().As<INotesDetailsListRepository>();
            builder.RegisterType<RecentlyViewedListHeadersRepositoryImpl>().As<IRecentlyViewedListHeadersRepository>();
            builder.RegisterType<RecentlyViewedListDetailsRepositoryImpl>().As<IRecentlyViewedListDetailsRepository>();
            builder.RegisterType<RecentlyOrderedListHeadersRepositoryImpl>().As<IRecentlyOrderedListHeadersRepository>();
            builder.RegisterType<RecentlyOrderedListDetailsRepositoryImpl>().As<IRecentlyOrderedListDetailsRepository>();
            builder.RegisterType<InventoryValuationListHeadersRepositoryImpl>().As<IInventoryValuationListHeadersRepository>();
            builder.RegisterType<InventoryValuationListDetailsRepositoryImpl>().As<IInventoryValuationListDetailsRepository>();
            builder.RegisterType<HistoryListHeaderRepositoryImpl>().As<IHistoryListHeaderRepository>();
            builder.RegisterType<HistoryListDetailRepositoryImpl>().As<IHistoryListDetailRepository>();
            builder.RegisterType<ContractListHeadersRepositoryImpl>().As<IContractListHeadersRepository>();
            builder.RegisterType<ContractListDetailsRepositoryImpl>().As<IContractListDetailsRepository>();
            builder.RegisterType<ReminderItemsListDetailsRepositoryImpl>().As<IRemindersListDetailsRepository>();
            builder.RegisterType<ReminderItemsListHeadersRepositoryImpl>().As<IRemindersListHeadersRepository>();
            builder.RegisterType<MandatoryItemsListHeadersRepositoryImpl>().As<IMandatoryItemsListHeadersRepository>();
            builder.RegisterType<MandatoryItemsListDetailsRepositoryImpl>().As<IMandatoryItemsListDetailsRepository>();
            builder.RegisterType<CustomListHeadersRepositoryImpl>().As<ICustomListHeadersRepository>();
            builder.RegisterType<CustomListDetailsRepositoryImpl>().As<ICustomListDetailsRepository>();
            builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
            builder.RegisterType<ListShareRepositoryImpl>().As<IListShareRepository>();
            builder.RegisterType<CustomInventoryItemsRepositoryImpl>().As<ICustomInventoryItemsRepository>();
            builder.RegisterType<CustomListSharesRepositoryImpl>().As<ICustomListSharesRepository>();
            builder.RegisterType<ContractChangesRepositoryImpl>().As<IContractChangesRepository>();

            // marketing
            builder.RegisterType<ContentManagementExternalRepositoryImpl>().As<IContentManagementExternalRepository>();
            builder.RegisterType<CampaignCustomerRepositoryImpl>()
                   .AsImplementedInterfaces();

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
            builder.RegisterType<OrderedFromListRepositoryImpl>().As<IOrderedFromListRepository>();
            builder.RegisterType<OrderedItemsFromListRepositoryImpl>().As<IOrderedItemsFromListRepository>();
            builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<IOrderSocketConnectionRepository>();
            builder.RegisterType<OrderUpdateRequestSocketRepositoryImpl>().As<IOrderUpdateSocketConnectionRepository>();
            builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();
            builder.RegisterType<SpecialOrderLogicImpl>().As<ISpecialOrderLogic>();
            builder.RegisterType<SpecialOrderRepositoryImpl>().As<ISpecialOrderRepository>();
            builder.RegisterType<SpecialOrderDBContext>().As<ISpecialOrderDBContext>();
            builder.RegisterType<OrderHistoryWriterImpl>().As<IOrderHistoryWriter>();
            builder.RegisterType<RecommendedItemsOrderedAnalyticsRepositoryImpl>().As<IRecommendedItemsOrderedAnalyticsRepository>();

            // other
            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();
            builder.RegisterType<DBAppSettingsRepositoryImpl>().As<IDBAppSettingsRepository>();
            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
            builder.Register(l => new EventLogQueueRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
            builder.RegisterType<ExportSettingRepositoryImpl>().As<IExportSettingRepository>();
            builder.RegisterType<PowerMenuRepositoryImpl>().As<IPowerMenuRepository>();
            builder.RegisterType<ReportRepository>().As<IReportRepository>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();
            builder.RegisterType<ApplicationHealthLogicImpl>().As<IApplicationHealthLogic>();
            builder.RegisterType<TemplatesRepositoryImpl>().As<ITemplatesRepository>();
            builder.RegisterType<ItemBarcodeImageRepositoryImpl>().As<IItemBarcodeImageRepository>();

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

            // user feedback 
            builder.RegisterType<UserFeedbackRepository>().As<IUserFeedbackRepository>();

            // FlipSnack API client
            builder.RegisterType<FlipSnackApiClient>().As<FlipSnackApiClient>();


            ///////////////////////////////////////////////////////////////////////////////
            // Logic Classes
            ///////////////////////////////////////////////////////////////////////////////

            // cart
            builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
            builder.RegisterType<ShoppingCartLogicImpl>().As<IShoppingCartLogic>();

            // catalog
            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();
            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            builder.RegisterType<SiteCatalogLogicImpl>().As<Entree.Core.Interface.SiteCatalog.ICatalogLogic>();
            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
            builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
            builder.RegisterType<UnfiImageProcessing>().As<IExternalImageProcessorUnfi>();

            // catalog campaign
            builder.RegisterType<CatalogCampaignLogicImpl>().As<ICatalogCampaignLogic>();

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
            builder.RegisterType<FavoritesListLogicImpl>().As<IFavoritesListLogic>();
            builder.RegisterType<NotesListLogicImpl>().As<INotesListLogic>();
            builder.RegisterType<ReminderItemsListLogicImpl>().As<IRemindersListLogic>();
            builder.RegisterType<MandatoryItemsListLogicImpl>().As<IMandatoryItemsListLogic>();
            builder.RegisterType<RecentlyViewedListLogicImpl>().As<IRecentlyViewedListLogic>();
            builder.RegisterType<RecentlyOrderedListLogicImpl>().As<IRecentlyOrderedListLogic>();
            builder.RegisterType<InventoryValuationListLogicImpl>().As<IInventoryValuationListLogic>();
            builder.RegisterType<ContractListLogicImpl>().As<IContractListLogic>();
            builder.RegisterType<ContractListChangesLogicImpl>().As<IContractListChangesLogic>();
            builder.RegisterType<HistoryListLogicImpl>().As<IHistoryListLogic>();
            builder.RegisterType<CustomListLogicImpl>().As<ICustomListLogic>();
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
            builder.RegisterType<UserFeedbackNotificationHandlerImpl>().Keyed<INotificationHandler>(NotificationType.UserFeedback);
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

            // user feedback 
            builder.RegisterType<UserFeedbackLogicImpl>().As<IUserFeedbackLogic>();


            ///////////////////////////////////////////////////////////////////////////////
            // Service Classes
            ///////////////////////////////////////////////////////////////////////////////

            // catalog campaign
            builder.RegisterType<CatalogCampaignServiceImpl>().As<ICatalogCampaignService>();
            builder.RegisterType<SiteCatalogServiceImpl>().As<ISiteCatalogService>();

            // invoices
            builder.RegisterType<ExportInvoicesServiceImpl>().As<IExportInvoicesService>();

            builder.RegisterType<ListServiceImpl>().As<IListService>();

            builder.RegisterType<ImportServiceImpl>().As<IImportService>();

            ///////////////////////////////////////////////////////////////////////////////
            // Environment Specific Classes
            ///////////////////////////////////////////////////////////////////////////////
#if DEMO
			builder.RegisterType<DemoExternalUserDomainRepositoryImpl>().As<ICustomerDomainRepository>();
			builder.RegisterType<DemoGenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
#else
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
            builder.RegisterType<GenericSubscriptionQueueRepositoryImpl>().As<IGenericSubscriptionQueueRepository>();
#endif


            // Services8

            // Shopping Cart
            builder.RegisterType<ShoppingCartServiceImpl>().As<IShoppingCartService>();

            builder.RegisterType<CacheListLogicImpl>().As<ICacheListLogic>();

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
