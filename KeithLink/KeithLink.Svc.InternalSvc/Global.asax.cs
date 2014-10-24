using Autofac;
using Autofac.Integration.Wcf;
using KeithLink.Common.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace KeithLink.Svc.InternalSvc
{
    public class Global : System.Web.HttpApplication
    {
        bool keepQueueListening = true;

        protected void Application_Start(object sender, EventArgs e)
        {
            IContainer container = AutofacContainerBuilder.BuildContainer();
            AutofacHostFactory.Container = container;
            //System.Threading.Tasks.Task.Factory.StartNew(() => this.QueueListener());
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

        protected void QueueListener()
        {
            while (keepQueueListening)
            {
                try
                {
                    // plug in RMQ listener here
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    IEventLogRepository eventLogRepository = ((IContainer)AutofacHostFactory.Container).Resolve<IEventLogRepository>();
                    eventLogRepository.WriteErrorLog("Error in Internal Service Queue Listener", ex);
                }
            }
        }
    }
}