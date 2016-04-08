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

            AutofacDependencyMapProvider.AddEventLogDependency(builder);
            AutofacDependencyMapProvider.AddStagingDependency(builder);
            AutofacDependencyMapProvider.AddOtherAccessServiceDependencies(builder);

            return builder;
        }

        public static ContainerBuilder GetInternalServiceContainer(){
            ContainerBuilder builder = new ContainerBuilder();

            //AutofacDependencyMapProvider.BuildBaselineDependencies(builder);
            AutofacDependencyMapProvider.AddOtherInternalServiceDependencies(builder);

            return builder;
        }

        public static ContainerBuilder GetOrderServiceContainer(){
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.AddEventLogDependency(builder);
            AutofacDependencyMapProvider.AddElasticSearchDependency(builder);
            AutofacDependencyMapProvider.AddGenericQueueDependency(builder);
            AutofacDependencyMapProvider.AddOtherOrderServiceDependencies(builder);

            return builder;
        }

        public static ContainerBuilder GetQueueSvcContainer() {
            ContainerBuilder builder = new ContainerBuilder();

            AutofacDependencyMapProvider.BuildBaselineDependencies(builder);
            AutofacDependencyMapProvider.AddOtherQueueServiceDependencies(builder);

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
