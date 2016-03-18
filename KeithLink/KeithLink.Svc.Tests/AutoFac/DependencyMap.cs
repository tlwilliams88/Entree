﻿#region Using
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
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.OnlinePayments.Log;
using KeithLink.Svc.Core.Interface.OnlinePayments.Payment;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Common.Impl.AuditLog;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.ContentManagement;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.SiteCatalog;
using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.Cache;
using KeithLink.Svc.Impl.Repository.ContentManagement;
using KeithLink.Svc.Impl.Repository.EF.Operational;
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
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.SiteCatalog;

using KeithLink.Svc.Test.Mock;

using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Svc.Impl.Logic.Profile;

#endregion

namespace KeithLink.Svc.Test
{
    internal static class DependencyMap
    {
        public static ContainerBuilder Init()
        {
            // Create the container builder.
            ContainerBuilder builder = new ContainerBuilder();

            return builder;
        }
        public static IContainer Build()
        {
            // Create the container builder.
            var builder = new ContainerBuilder();

			//*******************************************
			//Mock Items
			//*******************************************
            //builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWorkMock>().As<IUnitOfWork>().InstancePerLifetimeScope();

			//*******************************************
            //Logic Classes
			//*******************************************
            builder.RegisterType<ContentManagementLogicImpl>().As<IContentManagementLogic>();
			builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();
            builder.RegisterType<InternalDsrAliasLogicImpl>().As<IDsrAliasLogic>();
			builder.RegisterType<InternalListLogic>().As<IInternalListLogic>();
            builder.RegisterType<InternalMarketingPreferenceLogicImpl>().As<IInternalMarketingPreferenceLogic>();
			builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
			builder.RegisterType<SiteCatalogLogicImpl>().As<ICatalogLogic>();
            builder.RegisterType<InternalOnlinePaymentLogicImpl>().As<IOnlinePaymentsLogic>();
            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();
            builder.RegisterType<SettingsLogicImpl>().As<ISettingsLogicImpl>();
            builder.RegisterType<DsrLogic>().As<IDsrLogic>();
            builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            builder.RegisterType<InternalOrderHistoryLogic>().As<IInternalOrderHistoryLogic>();

			//*******************************************
			//Repositories
			//*******************************************
			//EF
            builder.RegisterType<KPayDBContext>().As<IKPayDBContext>();
			builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
			builder.RegisterType<ListItemRepositoryImpl>().As<IListItemRepository>();
			builder.RegisterType<ListShareRepositoryImpl>().As<IListShareRepository>();
			builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();
			builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();

            // Bill Pay
            builder.RegisterType<KPayInvoiceRepositoryImpl>().As<IKPayInvoiceRepository>();
            builder.RegisterType<CustomerBankRepositoryImpl>().As<ICustomerBankRepository>();
            builder.RegisterType<KPayLogRepositoryImpl>().As<IKPayLogRepository>();
            builder.RegisterType<KPayPaymentTransactionRepositoryImpl>().As<IKPayPaymentTransactionRepository>();

			//Catalog
			builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
			builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
			builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
			builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();

			//Etc
			builder.Register(c => new EventLogRepositoryImpl("Entree Test")).As<IEventLogRepository>();
			builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
			builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
            builder.RegisterType<ContentManagementExternalRepositoryImpl>().As<IContentManagementExternalRepository>();
            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();
            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();

            //Orders
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderConversionLogicImpl>().As<IOrderConversionLogic>();
            builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();
            
            //Profile
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<MarketingPreferencesRepositoryImpl>().As<IMarketingPreferencesRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
            builder.RegisterType<SettingsRepositoryImpl>().As<ISettingsRepository>();
            builder.RegisterType<DsrRepositoryImpl>().As<IDsrRepository>();
            
            //Replace
			builder.RegisterType<NoOrderServiceRepositoryImpl>().As<IOrderServiceRepository>();
			builder.RegisterType<NoListServiceRepositoryImpl>().As<IListServiceRepository>();
			builder.RegisterType<NoDsrServiceRepository>().As<IDsrServiceRepository>();
            builder.RegisterType<NoMessagingServiceRepositoryImpl>().As<IMessagingServiceRepository>();
            builder.RegisterType<NoInvoiceServiceRepositoryImpl>().As<IInvoiceServiceRepository>();
            builder.RegisterType<NoDsrAliasServiceImpl>().As<IDsrAliasService>();

            // Build the container.
            return builder.Build();
        }
        public static void RegisterStandard(this ContainerBuilder builder)
        {
            //*******************************************
            //Logic Classes
            //*******************************************
            builder.RegisterType<ContentManagementLogicImpl>().As<IContentManagementLogic>();
            builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();
            builder.RegisterType<InternalDsrAliasLogicImpl>().As<IDsrAliasLogic>();
            builder.RegisterType<InternalListLogic>().As<IInternalListLogic>();
            builder.RegisterType<InternalMarketingPreferenceLogicImpl>().As<IInternalMarketingPreferenceLogic>();
            builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
            builder.RegisterType<SiteCatalogLogicImpl>().As<ICatalogLogic>();
            builder.RegisterType<InternalOnlinePaymentLogicImpl>().As<IOnlinePaymentsLogic>();
            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();
            builder.RegisterType<SettingsLogicImpl>().As<ISettingsLogicImpl>();
            builder.RegisterType<DsrLogic>().As<IDsrLogic>();
            builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            builder.RegisterType<InternalOrderHistoryLogic>().As<IInternalOrderHistoryLogic>();

            //*******************************************
            //Repositories
            //*******************************************
            //EF
            builder.RegisterType<KPayDBContext>().As<IKPayDBContext>();
            builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
            builder.RegisterType<ListItemRepositoryImpl>().As<IListItemRepository>();
            builder.RegisterType<ListShareRepositoryImpl>().As<IListShareRepository>();
            builder.RegisterType<UserActiveCartRepositoryImpl>().As<IUserActiveCartRepository>();
            builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();

            // Bill Pay
            builder.RegisterType<KPayInvoiceRepositoryImpl>().As<IKPayInvoiceRepository>();
            builder.RegisterType<CustomerBankRepositoryImpl>().As<ICustomerBankRepository>();
            builder.RegisterType<KPayLogRepositoryImpl>().As<IKPayLogRepository>();
            builder.RegisterType<KPayPaymentTransactionRepositoryImpl>().As<IKPayPaymentTransactionRepository>();

            //Catalog
            builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
            builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();

            //Etc
            builder.Register(c => new EventLogRepositoryImpl("Entree Test")).As<IEventLogRepository>();
            builder.RegisterType<NoCacheRepositoryImpl>().As<ICacheRepository>();
            builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
            builder.RegisterType<ContentManagementExternalRepositoryImpl>().As<IContentManagementExternalRepository>();
            builder.RegisterType<AuditLogRepositoryImpl>().As<IAuditLogRepository>();
            builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();

            //Orders
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderConversionLogicImpl>().As<IOrderConversionLogic>();
            builder.RegisterType<PurchaseOrderRepositoryImpl>().As<IPurchaseOrderRepository>();

            //Profile
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<MarketingPreferencesRepositoryImpl>().As<IMarketingPreferencesRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
            builder.RegisterType<ItemHistoryRepositoryImpl>().As<IItemHistoryRepository>();
            builder.RegisterType<SettingsRepositoryImpl>().As<ISettingsRepository>();
            builder.RegisterType<DsrRepositoryImpl>().As<IDsrRepository>();

            //Replace
            builder.RegisterType<NoOrderServiceRepositoryImpl>().As<IOrderServiceRepository>();
            builder.RegisterType<NoListServiceRepositoryImpl>().As<IListServiceRepository>();
            builder.RegisterType<NoDsrServiceRepository>().As<IDsrServiceRepository>();
            builder.RegisterType<NoMessagingServiceRepositoryImpl>().As<IMessagingServiceRepository>();
            builder.RegisterType<NoInvoiceServiceRepositoryImpl>().As<IInvoiceServiceRepository>();
            builder.RegisterType<NoDsrAliasServiceImpl>().As<IDsrAliasService>();
        }
    }
}