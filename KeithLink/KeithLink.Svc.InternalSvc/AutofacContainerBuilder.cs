#region using__common
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Svc.Core.Interface.Configuration;
using KeithLink.Svc.Impl.Repository.Configurations;
using Autofac;
using Autofac.Features.Indexed;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.ETL;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Logic.SiteCatalog;
using KeithLink.Svc.Impl.Logic.Messaging;
using KeithLink.Svc.Core.Models.SiteCatalog;
#endregion
#region using__interface
using KeithLink.Svc.Core.Interface.Component;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Interface.Reports;
#endregion
#region using__repository
using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Email;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.InternalCatalog;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.OnlinePayments;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Customer;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Invoice;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.Confirmations;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Profile.Cache;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.Reports;
#endregion

namespace KeithLink.Svc.InternalSvc
{
    public static class AutofacContainerBuilder
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<DivisionService>();
            builder.RegisterType<ETLService>();
            builder.RegisterType<InvoiceService>();
            builder.RegisterType<ListServcie>();
            builder.RegisterType<MessagingService>();
            builder.RegisterType<OrderService>();
            builder.RegisterType<PipelineService>();
            builder.RegisterType<ContentManagementService>();
			builder.RegisterType<ConfigurationService>();
            builder.RegisterType<ReportService>();

            builder.RegisterType<CatalogInternalRepositoryImpl>().As<ICatalogInternalRepository>();
            builder.RegisterType<CatalogLogicImpl>().As<KeithLink.Svc.Core.ETL.ICatalogLogic>();
            builder.RegisterType<StagingRepositoryImpl>().As<IStagingRepository>();
            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();
            builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
            builder.RegisterType<InternalBasketRepository>().As<IInternalBasketRepository>();
            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<PriceCacheRepositoryImpl>().As<IPriceCacheRepository>();
            builder.RegisterType<CustomerLogicImpl>().As<ICustomerLogic>();
            builder.RegisterType<KeithLink.Svc.Impl.Logic.InternalSvc.InternalListLogic>().As<IInternalListLogic>();
            builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            //2nd builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            builder.RegisterType<UserProfileCacheRepository>().As<IUserProfileCacheRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();
            builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
            // 2nd builder.RegisterType<CustomerLogicImpl>().As <ICustomerLogic>();
            builder.RegisterType<NoCacheCustomerCacheRepositoryImpl>().As<ICustomerCacheRepository>();
            builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
            builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();
            builder.RegisterType<OrderUpdateQueueRepositoryImpl>().As<IOrderHistoryQueueRepository>();

            builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
            builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
            builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
            builder.RegisterType<CatalogCacheRepositoryImpl>().As<ICatalogCacheRepository>();
            builder.RegisterType<SiteCatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic>();
            builder.RegisterType<ListCachRepositoryImpl>().As<IListCacheRepository>();
            builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            builder.RegisterType<InternalOrderHistoryLogic>().As<IInternalOrderHistoryLogic>();
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();
            builder.RegisterType<InvoiceLogicImpl>().As<IInvoiceLogic>();
            builder.RegisterType<InternalInvoiceLogic>().As<IInternalInvoiceLogic>();
            builder.RegisterType<InvoiceRepositoryImpl>().As<IInvoiceRepository>();
            builder.RegisterType<InternalContentManagementLogic>().As<IInternalContentManagementLogic>();
            builder.RegisterType<ContentManagementItemRepositoryImpl>().As<IContentManagementItemRepository>();

            builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();
            builder.RegisterType<InvoiceItemRepositoryImpl>().As<IInvoiceItemRepository>();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
            builder.RegisterType<ListItemRepositoryImpl>().As<IListItemRepository>();

			builder.RegisterType<ListShareRepositoryImpl>().As<IListShareRepository>();

            //added 11-6
            builder.RegisterType<CustomerTopicRepositoryImpl>().As<ICustomerTopicRepository>();
            builder.RegisterType<KeithLink.Svc.Impl.Logic.InternalSvc.InternalMessagingLogic>().As<IInternalMessagingLogic>();
            builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
            builder.RegisterType<UserPushNotificationDeviceRepositoryImpl>().As<IUserPushNotificationDeviceRepository>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
            builder.RegisterType<NotificationQueueConsumerImpl>().As<INotificationQueueConsumer>();
            builder.RegisterType<AmazonPushNotificationMessageProvider>().As<IPushNotificationMessageProvider>();

            builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();
            builder.RegisterType<InternalDivisionLogic>().As<IInternalDivisionLogic>();
            builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();
            builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();
            builder.RegisterType<TokenReplacer>().As<ITokenReplacer>();
            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
            builder.RegisterType<MessageTemplateRepositoryImpl>().As<IMessageTemplateRepository>();
            builder.RegisterType<InternalOrderLogicImpl>().As<IInternalOrderLogic>();
            
            // keyed types - notification handlers
            builder.RegisterType<OrderConfirmationNotificationHandlerImpl>()
                .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.OrderConfirmation);
            builder.RegisterType<InvoiceNotificationHandlerImpl>()
                .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.InvoiceAttention);
            builder.RegisterType<EtaNotificationHandlerImpl>()
                .Keyed<INotificationHandler>(Svc.Core.Enumerations.Messaging.NotificationType.Eta);
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
                c =>
                {
                    var handlers = c.Resolve<IIndex<Svc.Core.Enumerations.Messaging.Channel, IMessageProvider>>();
                    return request => handlers[request];
                });

            // no implementation (will throw notimplementedexception if called)
            builder.RegisterType<NoOrderServiceRepositoryImpl>().As<IOrderServiceRepository>();
            builder.RegisterType<NoDivisionServiceRepositoryImpl>().As<IDivisionServiceRepository>();
            builder.RegisterType<NoListServiceRepositoryImpl>().As<IListServiceRepository>();
            builder.RegisterType<NoMessagingServiceRepositoryImpl>().As<IMessagingServiceRepository>();

			builder.RegisterType<TermRepositoryImpl>().As<ITermRepository>();
			builder.RegisterType<NoInvoiceServiceRepositoryImpl>().As<IInvoiceServiceRepository>();

            // customer bank - JA - 11/13
            builder.RegisterType<OnlinePaymentService>();
            builder.RegisterType<KPayDBContext>().As<IKPayDBContext>();
            builder.RegisterType<CustomerBankRepositoryImpl>().As<ICustomerBankRepository>();
            builder.RegisterType<NoOnlinePaymentServiceRepository>().As<IOnlinePaymentServiceRepository>();
            builder.RegisterType<KPayInvoiceRepositoryImpl>().As<IKPayInvoiceRepository>();

			builder.RegisterType<InternalOnlinePaymentLogicImpl>().As<IOnlinePaymentsLogic>();

			builder.RegisterType<InternalExportSettingsLogicImpl>().As<IInternalExportSettingLogic>();
			builder.RegisterType<ExportSettingRepositoryImpl>().As<IExportSettingRepository>();
			builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();

            builder.RegisterType<ReportRepository>().As<IReportRepository>();
            builder.RegisterType<InternalReportLogic>().As<IInternalReportLogic>();

            // order conversion - JA - 1/8/15
            builder.RegisterType<OrderConversionLogicImpl>().As<IOrderConversionLogic>();

            return builder.Build();
        }
	}
}