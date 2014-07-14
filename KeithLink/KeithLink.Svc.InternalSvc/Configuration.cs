using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Common.Core;

namespace KeithLink.Svc.InternalSvc
{
    public class Configuration : ConfigurationFacade
    {
        private const string KEY_SITE_NAME = "CS_SiteName";
        private const string KEY_STAGING_CONNECTIONSTRING = "StagingConnection";

        public static string CSSiteName
        {
            get { return GetValue(KEY_SITE_NAME, string.Empty); }
        }

        public static string StagingConnectionString
        {
            get { return GetConnectionString(KEY_STAGING_CONNECTIONSTRING); }
        }

    }
}