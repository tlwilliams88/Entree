using KeithLink.Common.Core.AuditLog;
using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.AuditLog;
using KeithLink.Common.Impl.Logging;

using KeithLink.Svc.Core.Interface.Cache;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Configuration;
using KeithLink.Svc.Core.Interface.ContentManagement;
using KeithLink.Svc.Core.Interface.Email;
using KeithLink.Svc.Core.Interface.ETL;
using KeithLink.Svc.Core.Interface.InternalCatalog;
using KeithLink.Svc.Core.Interface.Invoices;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Messaging;
using KeithLink.Svc.Core.Interface.OnlinePayments;
using KeithLink.Svc.Core.Interface.OnlinePayments.Customer;
using KeithLink.Svc.Core.Interface.OnlinePayments.Invoice;
using KeithLink.Svc.Core.Interface.OnlinePayments.Payment;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.Confirmations;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.Profile.PasswordReset;
using KeithLink.Svc.Core.Interface.Reports;
using KeithLink.Svc.Core.Interface.SiteCatalog;

using KeithLink.Svc.Impl;
using KeithLink.Svc.Impl.ETL;
using KeithLink.Svc.Impl.Logic;
using KeithLink.Svc.Impl.Logic.InternalSvc;
using KeithLink.Svc.Impl.Logic.Messaging;
using KeithLink.Svc.Impl.Logic.Orders;
using KeithLink.Svc.Impl.Logic.Profile;
using KeithLink.Svc.Impl.Logic.SiteCatalog;
using KeithLink.Svc.Impl.Repository.BranchSupports;
using KeithLink.Svc.Impl.Repository.Cache;
using KeithLink.Svc.Impl.Repository.Configurations;
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
using KeithLink.Svc.Impl.Repository.OnlinePayments.Payment;
using KeithLink.Svc.Impl.Repository.Orders;
using KeithLink.Svc.Impl.Repository.Orders.History;
using KeithLink.Svc.Impl.Repository.Orders.History.EF;
using KeithLink.Svc.Impl.Repository.Profile;
using KeithLink.Svc.Impl.Repository.Profile.PasswordReset;
using KeithLink.Svc.Impl.Repository.Queue;
using KeithLink.Svc.Impl.Repository.Reports;
using KeithLink.Svc.Impl.Repository.SiteCatalog;

using Autofac;
using Autofac.Features.Indexed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Windows.OrderService {
    public static class AutofacContainerBuilder {
        public static IContainer BuildContainer() {

            var builder = new ContainerBuilder();

            builder.Register(c => new EventLogRepositoryImpl(Configuration.ApplicationName)).As<IEventLogRepository>();

            builder.RegisterType<GenericQueueRepositoryImpl>().As<IGenericQueueRepository>();

            builder.RegisterType<ElasticSearchCatalogRepositoryImpl>().As<ICatalogRepository>();

            builder.RegisterType<OrderHistoryLogicImpl>().As<IOrderHistoryLogic>();
            builder.RegisterType<OrderHistoyrHeaderRepositoryImpl>().As<IOrderHistoryHeaderRepsitory>();
            builder.RegisterType<OrderHistoryDetailRepositoryImpl>().As<IOrderHistoryDetailRepository>();
            builder.RegisterType<OrderQueueLogicImpl>().As<IOrderQueueLogic>();
            builder.RegisterType<OrderHistoryRequestLogicImpl>().As<IOrderHistoryRequestLogic>();
            builder.RegisterType<ConfirmationLogicImpl>().As<IConfirmationLogic>();
            builder.RegisterType<SocketListenerRepositoryImpl>().As<ISocketListenerRepository>();
            builder.RegisterType<OrderSocketConnectionRepositoryImpl>().As<IOrderSocketConnectionRepository>();
            builder.RegisterType<OrderUpdateRequestSocketRepositoryImpl>().As<IOrderUpdateSocketConnectionRepository>();
            builder.RegisterType<NoOrderConversionLogicImpl>().As<IOrderConversionLogic>();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
