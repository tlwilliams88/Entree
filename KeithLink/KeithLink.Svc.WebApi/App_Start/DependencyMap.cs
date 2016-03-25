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
using KeithLink.Svc.Core.Interface.Export;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.OnlinePayments.Log;
using KeithLink.Svc.Core.Interface.OnlinePayments.Payment;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.Configurations;
using KeithLink.Svc.Impl.Logic.ContentManagement;
using KeithLink.Svc.Impl.Logic.Export;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Impl.Logic.Invoices;
using KeithLink.Svc.Impl.Logic.Messaging;
using KeithLink.Svc.Impl.Logic.OnlinePayments;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Logic.Reports;
using KeithLink.Svc.Impl.Logic.SiteCatalog;

using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.Brands;
using KeithLink.Svc.Impl.Repository.Cache;
using KeithLink.Svc.Impl.Repository.Configurations;
using KeithLink.Svc.Impl.Repository.ContentManagement;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Email;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Customer;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Invoice;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Log;
using KeithLink.Svc.Impl.Repository.OnlinePayments.Payment;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.Reports;
using KeithLink.Svc.Impl.Repository.SiteCatalog;

// this will be leaving soon!
using KeithLink.Svc.WebApi.Repository.Profile;

using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.WebApi {
    internal static class DependencyMap {
//        public static AutofacWebApiDependencyResolver Build(){
//            // Create the dependency resolver.
//            var resolver = new AutofacWebApiDependencyResolver(GetContainer());

//            return resolver;
//        }

//        public static IContainer GetContainer() {
//            // Create the container builder.
//            var builder = new ContainerBuilder();

//            // Register the Web API controllers.
//            builder.RegisterApiControllers(System.Reflection.Assembly.GetExecutingAssembly());

//            ///////////////////////////////////////////////////////////////////////////////
//            // Repositories
//            ///////////////////////////////////////////////////////////////////////////////

//            // cache
//            builder.RegisterType<CacheRepositoryImpl>().As<ICacheRepository>();
//            builder.RegisterType<CacheRefreshRepositoryImpl>().As<ICacheRefreshRepository>();

//            // cart
//            builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
//            builder.RegisterType<ShipDateRepositoryImpl>().As<IShipDateRepository>();

//            // catalog
//            builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();
//            builder.Register(b => new BrandRepositoryImpl()).As<IBrandRepository>().InstancePerRequest();
//            builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
//            builder.Register(c => new ElasticSearchCatalogRepositoryImpl()).As<ICatalogRepository>().InstancePerRequest();
//            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
//            builder.Register(pi => new ProductImageRepositoryImpl()).As<IProductImageRepository>().InstancePerRequest();
//            builder.RegisterType<ExternalCatalogRepositoryImpl>().As<IExternalCatalogRepository>();

//            // customer
//            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
//            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();

//            // division
//            builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();

//            // DSR
//            builder.RegisterType<DsrRepositoryImpl>().As<IDsrRepository>();

//            // invoices
//            builder.RegisterType<CustomerBankRepositoryImpl>().As<ICustomerBankRepository>();
//            builder.RegisterType<KPayDBContext>().As<IKPayDBContext>();
//            builder.RegisterType<KPayInvoiceRepositoryImpl>().As<IKPayInvoiceRepository>();
//            builder.RegisterType<KPayLogRepositoryImpl>().As<IKPayLogRepository>();
//            builder.RegisterType<KPayPaymentTransactionRepositoryImpl>().As<IKPayPaymentTransactionRepository>();
//            builder.RegisterType<TermRepositoryImpl>().As<ITermRepository>();

//            // lists
//            builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();

//            // marketing
//            builder.RegisterType<ContentManagementExternalRepositoryImpl>().As<IContentManagementExternalRepository>();

//            // messaging
//            builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
//            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
//            builder.RegisterType<UserPushNotificationDeviceRepositoryImpl>().As<IUserPushNotificationDeviceRepository>();
//            builder.RegisterType<AmazonPushNotificationMessageProvider>().As<IPushNotificationMessageProvider>();
//            builder.RegisterType<MessageTemplateRepositoryImpl>().As<IMessageTemplateRepository>();

//            // order
//            builder.RegisterType<ImagingRepositoryImpl>().As<IImagingRepository>();
//            builder.RegisterType<ItemHistoryRepositoryImpl>().As<IItemHistoryRepository>();
//            builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();
//            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
//            builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<IOrderSocketConnectionRepository>();
//            builder.RegisterType<OrderUpdateRequestSocketRepositoryImpl>().As<IOrderUpdateSocketConnectionRepository>();
//            builder.RegisterType<NoSpecialOrderRepositoryImpl>().As<ISpecialOrderRepository>();

//            // other
//            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();
//            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
//            builder.Register(l => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
//            builder.RegisterType<ExportSettingRepositoryImpl>().As<IExportSettingRepository>();
//            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();
//            builder.RegisterType<ReportRepository>().As<IReportRepository>();

//            // profile 
//            builder.RegisterType<AvatarRepositoryImpl>().As<IAvatarRepository>();
//            builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
//            builder.RegisterType<NoDsrAliasRepositoryImpl>().As<IDsrAliasRepository>();
//            //builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();  // this is also found in the DEMO preprocessor directive later
//            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
//            builder.RegisterType<MarketingPreferencesServiceRepositoryImpl>().As<IMarketingPreferencesServiceRepository>();
//            builder.RegisterType<NoSettingsRepositoryImpl>().As<ISettingsRepository>();
//            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
//            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();

//            ///////////////////////////////////////////////////////////////////////////////
//            // Logic Classes
//            ///////////////////////////////////////////////////////////////////////////////

//            // cart
//            builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
//            builder.RegisterType<ShoppingCartLogicImpl>().As<IShoppingCartLogic>();

//            // catalog
//            builder.RegisterType<SiteCatalogLogicImpl>().As<ICatalogLogic>();
//            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
            
//            // division
//            builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();

//            // DSR
//            builder.RegisterType<DsrLogic>().As<IDsrLogic>();

//            // invoices
//            builder.RegisterType<TermLogicImpl>().As<ITermLogic>();
//            builder.RegisterType<OnlinePaymentLogicImpl>().As<IOnlinePaymentsLogic>();

//            // lists
//            builder.RegisterType<InventoryValuationReportLogicImpl>().As<IInventoryValuationReportLogic>();

//            //marketing 
//            builder.RegisterType<ContentManagementLogicImpl>().As<IContentManagementLogic>();

//            // messaging
//            builder.RegisterType<MessagingLogicImpl>().As<IMessagingLogic>();
//            builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();

//            // order
//            builder.RegisterType<ImagingLogicImpl>().As<IImagingLogic>();
//            builder.RegisterType<ExportSettingLogicImpl>().As<IExportSettingLogic>();
//            builder.RegisterType<OrderHistoryRequestLogicImpl>().As<IOrderHistoryRequestLogic>();
//            builder.RegisterType<OrderLogicImpl>().As<IOrderLogic>();
//            builder.RegisterType<OrderQueueLogicImpl>().As<IOrderQueueLogic>();
            
//            // other
//            builder.RegisterType<ImportLogicImpl>().As<IImportLogic>();
//            builder.RegisterGeneric(typeof(ModelExportLogicImpl<>)).As(typeof(IModelExportLogic<>));
//            builder.RegisterType<ReportLogic>().As<IReportLogic>();

//            // profile
//            builder.RegisterType<NoDsrAliasLogicImpl>().As<IDsrAliasLogic>();
//            builder.RegisterType<NoSettingsLogicImpl>().As<ISettingsLogicImpl>();
//            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();

//            ///////////////////////////////////////////////////////////////////////////////
//            // still moving
//            ///////////////////////////////////////////////////////////////////////////////


//#if DEMO
//			builder.RegisterType<DemoExternalUserDomainRepositoryImpl>().As<ICustomerDomainRepository>();
//			builder.RegisterType<DemoGenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
//#else
//            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
//            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
//#endif

//            ///////////////////////////////////////////////////////////////////////////////
//            // service repositories
//            ///////////////////////////////////////////////////////////////////////////////
//            builder.RegisterType<Repository.Lists.ListServiceRepositoryImpl>().As<IListServiceRepository>();
//            builder.RegisterType<Repository.Orders.OrderServiceRepositoryImpl>().As<IOrderServiceRepository>();

//            builder.RegisterType<com.benekeith.ListService.ListServcieClient>().As<com.benekeith.ListService.IListServcie>();
//            builder.RegisterType<com.benekeith.OrderService.OrderServiceClient>().As<com.benekeith.OrderService.IOrderService>();
//            builder.RegisterType<com.benekeith.ProfileService.ProfileServiceClient>().As<com.benekeith.ProfileService.IProfileService>();
//            builder.RegisterType<PasswordResetServiceImpl>().As<IPasswordResetService>();
//            builder.RegisterType<DsrAliasServiceImpl>().As<IDsrAliasService>();

//            // Build the container.
//            return builder.Build();
//        }

        public static void AddServiceReferences(ref ContainerBuilder builder) {
            ///////////////////////////////////////////////////////////////////////////////
            // service repositories
            ///////////////////////////////////////////////////////////////////////////////
            builder.RegisterType<Repository.Lists.ListServiceRepositoryImpl>().As<IListServiceRepository>();
            builder.RegisterType<Repository.Orders.OrderServiceRepositoryImpl>().As<IOrderServiceRepository>();

            builder.RegisterType<com.benekeith.ListService.ListServcieClient>().As<com.benekeith.ListService.IListServcie>();
            builder.RegisterType<com.benekeith.OrderService.OrderServiceClient>().As<com.benekeith.OrderService.IOrderService>();
            builder.RegisterType<com.benekeith.ProfileService.ProfileServiceClient>().As<com.benekeith.ProfileService.IProfileService>();
            builder.RegisterType<PasswordResetServiceImpl>().As<IPasswordResetService>();
            builder.RegisterType<DsrAliasServiceImpl>().As<IDsrAliasService>();
            builder.RegisterType<MarketingPreferencesServiceRepositoryImpl>().As<IMarketingPreferencesServiceRepository>();
        }
    }
}