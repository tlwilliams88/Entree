﻿using Autofac;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.ETL;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Logic.SiteCatalog;
using KeithLink.Svc.Impl.Repository.EF.Operational;
using KeithLink.Svc.Impl.Repository.Lists;
using KeithLink.Svc.Impl.Repository.InternalCatalog;
using KeithLink.Svc.Impl.Repository.Messaging;
using KeithLink.Svc.Impl.Repository.Network;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.Confirmations;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Profile.Cache;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Impl.Repository.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Impl.Repository.Invoices;
using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Impl.Component;
using KeithLink.Svc.Core.Interface.Component;
using KeithLink.Svc.Impl.Repository.Email;


namespace KeithLink.Svc.InternalSvc
{
    public static class AutofacContainerBuilder
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ETLService>();
            builder.RegisterType<PipelineService>(); 
            builder.RegisterType<OrderService>();
			builder.RegisterType<ListServcie>();
			builder.RegisterType<InvoiceService>();
			builder.RegisterType<DivisionService>();
            builder.RegisterType<MessagingService>();

            builder.RegisterType<CatalogInternalRepositoryImpl>().As<ICatalogInternalRepository>();
            builder.RegisterType<CatalogLogicImpl>().As<KeithLink.Svc.Core.ETL.ICatalogLogic>();
            builder.RegisterType<StagingRepositoryImpl>().As<IStagingRepository>();
            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();
			builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
			builder.RegisterType<InternalBasketRepository>().As<IInternalBasketRepository>();
			builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
			builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
			builder.RegisterType<NoCachePriceCacheRepositoryImpl>().As<IPriceCacheRepository>();
            builder.RegisterType<CustomerLogicImpl>().As<ICustomerLogic>();
			builder.RegisterType<KeithLink.Svc.Impl.Logic.InternalSvc.InternalListLogic>().As<IInternalListLogic>();
            builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<UserProfileRepository>().As<IUserProfileRepository>();
            builder.RegisterType<ExternalUserDomainRepository>().As<ICustomerDomainRepository>();
            builder.RegisterType<InternalUserDomainRepository>().As<IUserDomainRepository>();
            builder.RegisterType<UserProfileCacheRepository>().As<IUserProfileCacheRepository>();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>();
            builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();
            builder.RegisterType<UserProfileLogicImpl>().As<IUserProfileLogic>();
            builder.RegisterType<CustomerContainerRepository>().As<ICustomerContainerRepository>();
            builder.RegisterType<CustomerLogicImpl>().As <ICustomerLogic>();
            builder.RegisterType<NoCacheCustomerCacheRepositoryImpl>().As<ICustomerCacheRepository>();
			builder.RegisterType<BasketLogicImpl>().As<IBasketLogic>();
            builder.RegisterType<OrderUpdateQueueRepositoryImpl>().As<IOrderHistoryQueueRepository>();
            builder.RegisterType<ConfirmationQueueRepositoryImpl>().As<IConfirmationQueueRepository>();
            builder.RegisterType<ProductImageRepositoryImpl>().As<IProductImageRepository>();
			builder.RegisterType<DivisionRepositoryImpl>().As<IDivisionRepository>();
			builder.RegisterType<CategoryImageRepository>().As<ICategoryImageRepository>();
			builder.RegisterType<CatalogCacheRepositoryImpl>().As<ICatalogCacheRepository>();
			builder.RegisterType<SiteCatalogLogicImpl>().As<KeithLink.Svc.Core.Interface.SiteCatalog.ICatalogLogic>();
			builder.RegisterType<ListCachRepositoryImpl>().As<IListCacheRepository>();
            builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>(); 
            builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>(); 
			builder.RegisterType<InvoiceLogicImpl>().As<IInvoiceLogic>();
			builder.RegisterType<InternalInvoiceLogic>().As<IInternalInvoiceLogic>();
			builder.RegisterType<InvoiceRepositoryImpl>().As<IInvoiceRepository>();
            builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();
            builder.RegisterType<ConfirmationQueueRepositoryImpl>().As<IQueueRepository>();
			builder.RegisterType<InvoiceItemRepositoryImpl>().As<IInvoiceItemRepository>();
			builder.RegisterType<InternalInvoiceLogic>().As<IInternalInvoiceLogic>();
			builder.RegisterType<InvoiceRepositoryImpl>().As<IInvoiceRepository>();
			builder.RegisterType<InvoiceItemRepositoryImpl>().As<IInvoiceItemRepository>();

			builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
			builder.RegisterType<ListRepositoryImpl>().As<IListRepository>();
			builder.RegisterType<ListItemRepositoryImpl>().As<IListItemRepository>();

            builder.RegisterType<CustomerTopicRepositoryImpl>().As<ICustomerTopicRepository>();
            builder.RegisterType<KeithLink.Svc.Impl.Logic.InternalSvc.InternalMessagingLogic>().As<IInternalMessagingLogic>();
            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();

			builder.RegisterType<ListServiceRepositoryImpl>().As<IListServiceRepository>();
			builder.RegisterType<KeithLink.Svc.Impl.com.benekeith.ListService.ListServcieClient>().As<KeithLink.Svc.Impl.com.benekeith.ListService.IListServcie>();
			builder.RegisterType<KeithLink.Svc.Impl.com.benekeith.DivisionService.DivisionServiceClient>().As<KeithLink.Svc.Impl.com.benekeith.DivisionService.IDivisionService>();

            builder.RegisterType<MessagingServiceRepositoryImpl>().As<IMessagingServiceRepository>();
            builder.RegisterType<KeithLink.Svc.Impl.com.benekeith.MessagingService.MessagingServiceClient>().As<KeithLink.Svc.Impl.com.benekeith.MessagingService.IMessagingService>();
			builder.RegisterType<DivisionLogicImpl>().As<IDivisionLogic>();

			builder.RegisterType<InternalDivisionLogic>().As<IInternalDivisionLogic>();
			builder.RegisterType<DivisionServiceRepositoryImpl>().As<IDivisionServiceRepository>();
			
			builder.RegisterType<BranchSupportRepositoryImpl>().As<IBranchSupportRepository>();		
			builder.RegisterType<MessageTemplateLogicImpl>().As<IMessageTemplateLogic>();
			builder.RegisterType<TokenReplacer>().As<ITokenReplacer>();
			builder.RegisterType<EmailClientImpl>().As<IEmailClient>();
			builder.RegisterType<MessageTemplateRepositoryImpl>().As<IMessageTemplateRepository>();
            builder.RegisterType<UserMessageRepositoryImpl>().As<IUserMessageRepository>();
            builder.RegisterType<UserMessagingPreferenceRepositoryImpl>().As<IUserMessagingPreferenceRepository>();
            return builder.Build();
        }

		
	}
}