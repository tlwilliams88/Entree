using Autofac;
using KeithLink.Svc.Core.Enumerations.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.SmartResolver
{
    public static class DependencyMapFactory
    {
        public static ContainerBuilder GetAccessServiceContainer(){
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.BuildBaselineDependencies(builder);
            AutofacDependencyMapProvider.AddOtherAccessServiceDependencies(builder);

            return builder;
        }

        public static ContainerBuilder GetInternalServiceContainer(){
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.BuildBaselineDependencies(builder);
            AutofacDependencyMapProvider.AddOtherInternalServiceDependencies(builder);

            return builder;
        }

        public static ContainerBuilder GetCatalogSvcContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.BuildBaselineDependencies(builder);
            AutofacDependencyMapProvider.AddOtherCatalogServiceDependencies(builder);

            return builder;
        }

        public static ContainerBuilder GetOrderServiceContainer(){
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.BuildBaselineDependencies(builder);
            AutofacDependencyMapProvider.AddOtherOrderServiceDependencies(builder);

            return builder;
        }

        public static ContainerBuilder GetQueueSvcContainer(DependencyInstanceType type = DependencyInstanceType.InstancePerLifetimeScope) {
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.BuildBaselineDependencies(builder, type);
            AutofacDependencyMapProvider.AddOtherQueueServiceDependencies(builder);

            return builder;
        }

        public static ContainerBuilder GetMonitorSvcContainer(DependencyInstanceType type = DependencyInstanceType.InstancePerLifetimeScope)
        {
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.BuildBaselineDependencies(builder, type);
            AutofacDependencyMapProvider.AddOtherMonitorServiceDependencies(builder);

            return builder;
        }

        public static ContainerBuilder GetTestsContainer(DependencyInstanceType type = DependencyInstanceType.SingleInstance)
        {
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.BuildBaselineDependencies(builder, type);

            return builder;
        }

        public static ContainerBuilder GetWebApiContainer() {
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.BuildBaselineDependencies(builder);
            AutofacDependencyMapProvider.AddOtherWebApiDependencies(builder);

            return builder;
        }
    }
}
