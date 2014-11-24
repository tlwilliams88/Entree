using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Diagnostics;

namespace KeithLink.Svc.FoundationSvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                KeithLink.Common.Impl.Logging.EventLogRepositoryImpl eventLog =
                    new Common.Impl.Logging.EventLogRepositoryImpl("KeithLink.FoundationSvc");
                eventLog.WriteInformationLog("Foundation Service Starting Up");
            }
            catch (Exception ex)
            {
                string sSource = "KeithLink.FoundationSvc";
                string sLog = "Application";
                string sEvent = "Error trying to log in startup: " + ex.ToString();

                if (!EventLog.SourceExists(sSource))
	                EventLog.CreateEventSource(sSource,sLog);
					
                EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error,  234);
            }
        }
    }
}
