using Autofac;
using KeithLink.Svc.Core.ETL;
using KeithLink.Svc.Impl.ETL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KeithLink.Svc.InternalSvc
{
    public static class AutofacContainerBuilder
    {
        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ETLService>();
            builder.RegisterType<CategoryLogicImpl>().As<ICategoryLogic>();

            return builder.Build();
        }
    }
}