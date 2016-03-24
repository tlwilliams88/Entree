

using Autofac;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.SmartResolver
{
    public static class DependencyMapFactory
    {
        public static ContainerBuilder BuildInternalServiceContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            AutofacDependencyMapProvider.AddOtherInternalServiceDependencies(builder);
            return builder;
        }
        public static ContainerBuilder BuildWebApiContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            AutofacDependencyMapProvider.AddOtherWebApiDependencies(builder);
            return builder;
        }
        public static ContainerBuilder BuildQueueSvcContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            AutofacDependencyMapProvider.AddEventLogDependency(builder);
            AutofacDependencyMapProvider.AddStagingDependency(builder);
            AutofacDependencyMapProvider.AddElasticSearchDependency(builder);
            AutofacDependencyMapProvider.AddOtherQueueServiceDependencies(builder);
            return builder;
        }
        public static ContainerBuilder BuildAccessServiceContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            AutofacDependencyMapProvider.AddEventLogDependency(builder);
            AutofacDependencyMapProvider.AddStagingDependency(builder);
            AutofacDependencyMapProvider.AddOtherAccessServiceDependencies(builder);
            return builder;
        }
        public static ContainerBuilder BuildOrderServiceContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            AutofacDependencyMapProvider.AddEventLogDependency(builder);
            AutofacDependencyMapProvider.AddElasticSearchDependency(builder);
            AutofacDependencyMapProvider.AddGenericQueueDependency(builder);
            AutofacDependencyMapProvider.AddOtherOrderServiceDependencies(builder);
            return builder;
        }
    }
}
