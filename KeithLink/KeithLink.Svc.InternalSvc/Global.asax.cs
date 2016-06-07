using KeithLink.Svc.Impl.Repository.SmartResolver;

using Autofac;
using Autofac.Integration.Wcf;
using System;

namespace KeithLink.Svc.InternalSvc
{
    public class Global : System.Web.HttpApplication {
       #region events
        protected void Application_Start(object sender, EventArgs e)
        {
            ContainerBuilder container = DependencyMapFactory.GetInternalServiceContainer();
            AutofacContainerBuilder.AddServiceReferences(ref container);

            AutofacHostFactory.Container = container.Build();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
           
        }
        #endregion		       
	}
}