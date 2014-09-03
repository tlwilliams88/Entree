using Autofac;
using KeithLink.Svc.Core;
using KeithLink.Svc.Core.ETL;
using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Impl.Repository.InternalCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Core.Interface.Orders;

namespace KeithLink.Svc.InternalSvc
{
    public static class AutofacContainerBuilder
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ETLService>();
			builder.RegisterType<OrderService>();
            builder.RegisterType<CatalogInternalRepositoryImpl>().As<ICatalogInternalRepository>();
            builder.RegisterType<CatalogLogicImpl>().As<ICatalogLogic>();
            builder.RegisterType<StagingRepositoryImpl>().As<IStagingRepository>();
            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();
			builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
			builder.RegisterType<InternalBasketRepository>().As<IInternalBasketRepository>();

            return builder.Build();
        }

		
	}
}