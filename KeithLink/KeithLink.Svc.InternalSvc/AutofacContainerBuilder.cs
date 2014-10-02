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
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Repository.SiteCatalog;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Lists;

namespace KeithLink.Svc.InternalSvc
{
    public static class AutofacContainerBuilder
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ETLService>();
			builder.RegisterType<PipelineService>();
            builder.RegisterType<CatalogInternalRepositoryImpl>().As<ICatalogInternalRepository>();
            builder.RegisterType<CatalogLogicImpl>().As<KeithLink.Svc.Core.ETL.ICatalogLogic>();
            builder.RegisterType<StagingRepositoryImpl>().As<IStagingRepository>();
            builder.RegisterType<ElasticSearchRepositoryImpl>().As<IElasticSearchRepository>();
			builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();
			builder.RegisterType<InternalBasketRepository>().As<IInternalBasketRepository>();
			builder.RegisterType<PriceLogicImpl>().As<IPriceLogic>();
			builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
			builder.RegisterType<NoCachePriceCacheRepositoryImpl>().As<IPriceCacheRepository>();
            /*new stuff*/
            builder.RegisterType<ListLogicImpl>().As<IListLogic>();
            builder.RegisterType<BasketRepositoryImpl>().As<IBasketRepository>();
            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();
            builder.RegisterType<PriceRepositoryImpl>().As<IPriceRepository>();
            builder.RegisterType<ItemNoteLogicImpl>().As<IItemNoteLogic>();

            
            /*end new stuff*/
            return builder.Build();
        }

		
	}
}