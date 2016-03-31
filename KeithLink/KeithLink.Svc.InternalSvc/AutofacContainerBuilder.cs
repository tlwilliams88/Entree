using KeithLink.Common.Core.AuditLog;
using KeithLink.Common.Core.Logging;

using KeithLink.Common.Impl.AuditLog;
using KeithLink.Common.Impl.Logging;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configurations;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.ETL.ElasticSearch;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Svc.Core.Interface.Invoices;
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
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.ETL;

using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.Configurations;
using KeithLink.Svc.Impl.Logic.ETL;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Impl.Logic.Invoices;
using KeithLink.Svc.Impl.Logic.Messaging;
using KeithLink.Svc.Impl.Logic.OnlinePayments;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.PowerMenu;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Logic.SiteCatalog;

using KeithLink.Svc.Impl.Repository.Cache;
using KeithLink.Svc.Impl.Repository.Configurations;
using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Email;
using KeithLink.Svc.Impl.Repository.InternalCatalog;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.OnlinePayments;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Customer;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Invoice;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Log;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Payment;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.PowerMenu;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Profile.PasswordReset;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.Reports;
using KeithLink.Svc.Impl.Repository.SiteCatalog;

using Autofac;
using Autofac.Features.Indexed;

using System;

namespace KeithLink.Svc.InternalSvc
{
    public static class AutofacContainerBuilder
    {
        public static void AddServiceReferences(ref ContainerBuilder builder) {
            builder.RegisterType<ETLService>();
            builder.RegisterType<PipelineService>();
            builder.RegisterType<CacheService>();
            builder.RegisterType<ProfileService>();
        }

   //     public static IContainer BuildContainer()
   //     {
   //         var builder = new ContainerBuilder();

   //         builder.RegisterType<ETLService>();
   //         builder.RegisterType<ListServcie>();
   //         builder.RegisterType<OrderService>();
   //         builder.RegisterType<PipelineService>();
			//builder.RegisterType<CacheService>();
			//builder.RegisterType<ProfileService>();

			//#if DEMO
			//	builder.RegisterType<DemoStagingRepositoryImpl>().As<IStagingRepository>();
			//	builder.RegisterType<DemoGenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
			//#else
			//	builder.RegisterType<StagingRepositoryImpl>().As<IStagingRepository>();
			//	builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
			//#endif


			//builder.RegisterType<CatalogInternalRepositoryImpl>().As<ICatalogInternalRepository>();
   //         builder.RegisterType<CatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.ETL.ICatalogLogic>();
            
   //         builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();
   //         builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
   //         builder.RegisterType<InternalBasketRepository>().As<IInternalBasketRepository>();
   //         builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
   //         builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
   //         builder.RegisterType<CustomerLogicImpl>().As<ICustomerLogic>();
   //         builder.RegisterType<KeithLink.Svc.Impl.Logic.InternalSvc.InternalListLogic>().As<IInternalListLogic>();
   //         builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
   //         builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
   //         //2nd builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
   //         builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
   //         builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
   //         builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
   //         builder.RegisterType<AccountRepository>().As<IAccountRepository>();
   //         builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
   //         builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();
   //         builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
   //         // 2nd builder.RegisterType<CustomerLogicImpl>().As <ICustomerLogic>();
   //         builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
   //         builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();

   //         builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
   //         builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
   //         builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
   //         builder.RegisterType<SiteCatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic>();
   //         builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
   //         builder.RegisterType<InternalOrderHistoryLogic>().As<IInternalOrderHistoryLogic>();
   //         builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
   //         builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();
   //         //builder.RegisterType<InternalContentManagementLogic>().As<IInternalContentManagementLogic>();
   //         //builder.RegisterType<ContentManagementItemRepositoryImpl>().As<IContentManagementItemRepository>();

   //         builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
   //         builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();

   //         builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
   //         builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
   //         builder.RegisterType<ListItemRepositoryImpl>().As<IListItemRepository>();

			//builder.RegisterType<ListShareRepositoryImpl>().As<IListShareRepository>();

   //         //added 11-6
   //         builder.RegisterType<CustomerTopicRepositoryImpl>().As<ICustomerTopicRepository>();
   //         builder.RegisterType<MessagingLogicImpl>().As<IMessagingLogic>();
   //         builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
   //         builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
   //         builder.RegisterType<UserPushNotificationDeviceRepositoryImpl>().As<IUserPushNotificationDeviceRepository>();
            
   //         builder.RegisterType<NotificationQueueConsumerImpl>().As<INotificationQueueConsumer>();
   //         builder.RegisterType<AmazonPushNotificationMessageProvider>().As<IPushNotificationMessageProvider>();

   //         builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();
   //         builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();
   //         builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();
   //         builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
   //         builder.RegisterType<MessageTemplateRepositoryImpl>().As<IMessageTemplateRepository>();
   //         builder.RegisterType<InternalOrderLogicImpl>().As<IInternalOrderLogic>();
            
   //         // keyed types - notification handlers
   //         builder.RegisterType<OrderConfirmationNotificationHandlerImpl>()
   //             .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.OrderConfirmation);
   //         builder.RegisterType<InvoiceNotificationHandlerImpl>()
   //             .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.InvoiceAttention);
   //         builder.RegisterType<EtaNotificationHandlerImpl>()
   //             .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.Eta);
   //         builder.RegisterType<PaymentConfirmationNotificationHandlerImpl>()
   //             .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.PaymentConfirmation);
			//builder.RegisterType<HasNewsNotificationHandlerImpl>()
			//	.Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.HasNews);
   //         builder.Register<Func<Svc.Core.Enumerations.Messaging.NotificationType, INotificationHandler>>(
   //             c => {
   //                 var handlers = c.Resolve<IIndex<Svc.Core.Enumerations.Messaging.NotificationType, INotificationHandler>>();
   //                 return request => handlers[request];  
   //             });

   //         // keyed types - message providers
   //         builder.RegisterType<WebMessageProvider>()
   //             .Keyed<IMessageProvider>(Svc.Core.Enumerations.Messaging.Channel.Web);
   //         builder.RegisterType<EmailMessageProvider>()
   //             .Keyed<IMessageProvider>(Svc.Core.Enumerations.Messaging.Channel.Email);
   //         builder.RegisterType<AmazonPushNotificationMessageProvider>()
   //             .Keyed<IMessageProvider>(Svc.Core.Enumerations.Messaging.Channel.MobilePush);
   //         builder.Register<Func<Svc.Core.Enumerations.Messaging.Channel, IMessageProvider>>(
   //             c =>
   //             {
   //                 var handlers = c.Resolve<IIndex<Svc.Core.Enumerations.Messaging.Channel, IMessageProvider>>();
   //                 return request => handlers[request];
   //             });

   //         // no implementation (will throw notimplementedexception if called)
   //         builder.RegisterType<NoOrderServiceRepositoryImpl>().As<IOrderServiceRepository>();
   //         builder.RegisterType<NoListServiceRepositoryImpl>().As<IListServiceRepository>();

			//builder.RegisterType<TermRepositoryImpl>().As<ITermRepository>();
   //         builder.RegisterType<TermLogicImpl>().As<ITermLogic>();

   //         // customer bank - JA - 11/13<
   //         builder.RegisterType<KPayDBContext>().As<IKPayDBContext>();
   //         builder.RegisterType<CustomerBankRepositoryImpl>().As<ICustomerBankRepository>();
   //         builder.RegisterType<KPayInvoiceRepositoryImpl>().As<IKPayInvoiceRepository>();
   //         builder.RegisterType<KPayLogRepositoryImpl>().As<IKPayLogRepository>();
			//builder.RegisterType<KPayPaymentTransactionRepositoryImpl>().As<IKPayPaymentTransactionRepository>();
			//builder.RegisterType<OnlinePaymentLogicImpl>().As<IOnlinePaymentsLogic>();

			//builder.RegisterType<ExportSettingRepositoryImpl>().As<IExportSettingRepository>();
   //         builder.RegisterType<ExportSettingLogicImpl>().As<IExportSettingLogic>();
   //         builder.RegisterType<ExternalCatalogRepositoryImpl>().As<IExternalCatalogRepository>();
			//builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();

   //         builder.RegisterType<ReportRepository>().As<IReportRepository>();

   //         // order conversion - JA - 1/8/15
   //         builder.RegisterType<OrderConversionLogicImpl>().As<IOrderConversionLogic>();

			//builder.RegisterType<CacheRepositoryImpl>().As<ICacheRepository>();
			//builder.RegisterType<CacheRefreshRepositoryImpl>().As<ICacheRefreshRepository>();


   //         // dsr repository
   //         builder.RegisterType<DsrRepositoryImpl>().As<IDsrRepository>();
   //         builder.RegisterType<DsrLogic>().As<IDsrLogic>();


   //         // PowerMenu
   //         builder.RegisterType<PowerMenuRepositoryImpl>().As<IPowerMenuRepository>();
   //         builder.RegisterType<PowerMenuLogicImpl>().As<IPowerMenuLogic>();

			////Password Reset
			//builder.RegisterType<InternalPasswordResetRequestLogicImpl>().As<IInternalPasswordResetLogic>();
			//builder.RegisterType<PasswordResetRequestRepositoryImpl>().As<IPasswordResetRequestRepository>();

   //         // DSR Alias
   //         builder.RegisterType<DsrAliasRepositoryImpl>().As<IDsrAliasRepository>();
   //         builder.RegisterType<NoDsrAliasServiceImpl>().As<IDsrAliasService>();
			//builder.RegisterType<InternalDsrAliasLogicImpl>().As<IDsrAliasLogic>();

			//builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();

			//builder.RegisterType<NoPasswordResetServiceRepositoryImpl>().As<IPasswordResetService>();

			//builder.RegisterType<MarketingPreferencesRepositoryImpl>().As<IMarketingPreferencesRepository>();
			//builder.RegisterType<InternalMarketingPreferenceLogicImpl>().As<IInternalMarketingPreferenceLogic>();

   //         // ElasticSearch ETL 
   //         builder.RegisterType<ItemImportLogicImpl>().As<IItemImport>();
   //         builder.RegisterType<CategoriesImportLogicImpl>().As<ICategoriesImport>();
   //         builder.RegisterType<HouseBrandsImportLogicImpl>().As<IHouseBrandsImport>();

   //         // List ETL
   //         builder.RegisterType<ListImportLogicImpl>().As<IListsImportLogic>();

   //         // Item History
   //         builder.RegisterType<ItemHistoryRepositoryImpl>().As<IItemHistoryRepository>();

   //         // Profile Settings
   //         builder.RegisterType<SettingsRepositoryImpl>().As<ISettingsRepository>();
   //         builder.RegisterType<SettingsLogicImpl>().As<ISettingsLogicImpl>();

   //         // messaging
   //         builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
   //         builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
   //         builder.RegisterType<UserPushNotificationDeviceRepositoryImpl>().As<IUserPushNotificationDeviceRepository>();
   //         builder.RegisterType<AmazonPushNotificationMessageProvider>().As<IPushNotificationMessageProvider>();
   //         builder.RegisterType<MessagingLogicImpl>().As<IMessagingLogic>();
   //         builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();


   //         return builder.Build();
   //     }
	}
}