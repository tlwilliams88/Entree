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
        public static string KEY_CORS_ENABLED_DOMAINS { get { return "CorsEnabledDomains"; } }
        public static string KEY_CORS_ENABLED_HEADERS { get { return "CorsEnabledHeaders"; } }
        public static string KEY_CORS_ENABLED_METHODS { get { return "CorsEnabledMethods"; } }
        public static string KEY_DEFAULT_CATEGORY_RETURN_SIZE { get { return "DefaultCategoryReturnSize"; } }
        public static string KEY_DEFAULT_PRODUCT_RETURN_SIZE { get { return "DefaultProductReturnSize"; } }
        private const string DEFAULT_ELASTIC_SEARCH_BATCH_SIZE = "500";
        private const string DEFAULT_CATEGORY_RETURN_SIZE = "2000";
        private const string DEFAULT_PRODUCT_RETURN_SIZE = "500";

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

        public static string CorsEnabledDomains
        {
            get { return GetValue(KEY_CORS_ENABLED_DOMAINS, string.Empty); }
        }

        public static string CorsEnabledHeaders
        {
            get { return GetValue(KEY_CORS_ENABLED_HEADERS, string.Empty); }
        }

        public static string CorsEnabledMethods
        {
            get { return GetValue(KEY_CORS_ENABLED_METHODS, string.Empty); }
        }

        public static int DefaultCategoryReturnSize
        {
            get 
            { 
                string value = GetValue(KEY_DEFAULT_CATEGORY_RETURN_SIZE, DEFAULT_CATEGORY_RETURN_SIZE);
                return ValueParsingUtil.ParseInt(value, DEFAULT_CATEGORY_RETURN_SIZE);
            }
        }

        public static int DefaultProductReturnSize
        {
            get
            { 
                string value = GetValue(KEY_DEFAULT_PRODUCT_RETURN_SIZE, DEFAULT_PRODUCT_RETURN_SIZE);
                return ValueParsingUtil.ParseInt(value, DEFAULT_PRODUCT_RETURN_SIZE);
            }
        }
    }
}