using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Common.Core;

namespace KeithLink.Svc.Impl
{
    public class Configuration : ConfigurationFacade
    {
        private const string KEY_SITE_NAME = "CS_SiteName";
        private const string KEY_STAGING_CONNECTIONSTRING = "StagingConnection";
        private const string KEY_BASE_CATALOG = "CS_BaseCatalog";
        private const string KEY_ELASTIC_SEARCH_URL = "ElasticSearchURL";

        public static string ElasticSearchURL
        {
            get { return GetValue(KEY_ELASTIC_SEARCH_URL, string.Empty); }
        }

        public static string CSSiteName
        {
            get { return GetValue(KEY_SITE_NAME, string.Empty); }
        }

        public static string BaseCatalog
        {
            get { return GetValue(KEY_BASE_CATALOG, string.Empty); }
        }

        public static string StagingConnectionString
        {
            get { return GetConnectionString(KEY_STAGING_CONNECTIONSTRING); }
        }

    }
}