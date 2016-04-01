using Autofac;

namespace KeithLink.Svc.InternalSvc
{
    public static class AutofacContainerBuilder
    {
        public static void AddServiceReferences(ref ContainerBuilder builder) {
            builder.RegisterType<ETLService>();
            builder.RegisterType<PipelineService>();
            builder.RegisterType<CacheService>();
        }
	}
}