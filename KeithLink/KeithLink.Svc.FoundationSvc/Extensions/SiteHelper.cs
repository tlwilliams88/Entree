using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommerceServer.Foundation;
using CommerceServer.Foundation.SequenceComponents.Utility;

namespace KeithLink.Svc.FoundationSvc.Extensions
{
    public class SiteHelper
    {
        public static string GetSiteName()
        {
            if (CommerceOperationContext.CurrentInstance == null)
            {
                throw CommonExceptions.MissingOperationContext();
            }
            return CommerceOperationContext.CurrentInstance.SiteName;
        }

        static CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection siteConfig = null;
        public static CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection GetCsConfig()
        {
            if (siteConfig == null)
            {
                siteConfig = new CommerceServer.Core.Runtime.Configuration.CommerceResourceCollection(SiteHelper.GetSiteName());
            }
            return siteConfig;
        }
    }
}