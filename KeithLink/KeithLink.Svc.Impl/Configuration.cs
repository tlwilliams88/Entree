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
        private const string KEY_APPDATA_CONNECTIONSTRING = "AppDataConnection";
        private const string KEY_BASE_CATALOG = "CS_BaseCatalog";
        private const string KEY_ELASTIC_SEARCH_URL = "ElasticSearchURL";
        private const string KEY_ELASTIC_SEARCH_BATCH_SIZE = "ElasticSearchBatchSize";
        private const string DEFAULT_ELASTIC_SEARCH_BATCH_SIZE = "500";

        public static int ElasticSearchBatchSize
        {
            get
            {
                string value = GetValue(KEY_ELASTIC_SEARCH_BATCH_SIZE, DEFAULT_ELASTIC_SEARCH_BATCH_SIZE);
                return ValueParsingUtil.ParseInt(value, DEFAULT_ELASTIC_SEARCH_BATCH_SIZE);
            }
        }

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

        public static string AppDataConnectionString
        {
            get { return GetConnectionString(KEY_APPDATA_CONNECTIONSTRING); }
        }

    }
}