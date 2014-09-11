using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KeithLink.Common.Core;

namespace KeithLink.Svc.Impl
{
    public class Configuration : ConfigurationFacade
    {
        #region attributes
        private const string DEFAULT_APPNAME = "KeithCom";
        private const string DEFAULT_CATEGORY_RETURN_SIZE = "2000";
        private const string DEFAULT_ELASTIC_SEARCH_BATCH_SIZE = "500";
        private const string DEFAULT_PRODUCT_RETURN_SIZE = "500";
        private const string DEFAULT_MAX_SORT_BY_PRICE_ITEM_COUNT = "200";
        private const string KEY_AD_EXTERNAL_DOMAIN = "ADExtDomain";
        private const string KEY_AD_EXTERNAL_PASSWORD = "ADExtPass";
        private const string KEY_AD_EXTERNAL_ROOTNODE = "ADExtRoot";
        private const string KEY_AD_EXTERNAL_SERVERNAME = "ADExtServer";
        private const string KEY_AD_EXTERNAL_USER = "ADExtUser";
        private const string KEY_AD_INTERNAL_DOMAIN = "ADIntDomain";
        private const string KEY_AD_INTERNAL_PASSWORD = "ADIntPass";
        private const string KEY_AD_INTERNAL_ROOTNODE = "ADIntRoot";
        private const string KEY_AD_INTERNAL_SERVERNAME = "ADIntServer";
        private const string KEY_AD_INTERNAL_USER = "ADIntUser";
        private const string KEY_AD_INVALIDATTEMPTS = "ADBadPwdCount";
        private const string KEY_AD_LOCKOUTDURATION = "ADLockoutDuration";
        private const string KEY_APP_NAME = "AppName";
        private const string KEY_ALLOWED_API_KEYS = "AllowedApiKeys";
        private const string KEY_APPDATA_CONNECTIONSTRING = "AppDataConnection";
        private const string KEY_BASE_CATALOG = "CS_BaseCatalog";
        private const string KEY_BRAND_ASSETS_URL = "BrandAssetsUrl";
        private const string KEY_CATEGORY_PREFIXES = "CategoryPrefixesToExclude";
        private const string KEY_CORS_ENABLED_DOMAINS = "CorsEnabledDomains";
        private const string KEY_CORS_ENABLED_HEADERS = "CorsEnabledHeaders";
        private const string KEY_CORS_ENABLED_METHODS = "CorsEnabledMethods";
        private const string KEY_DEFAULT_CATEGORY_RETURN_SIZE = "DefaultCategoryReturnSize";
        private const string KEY_DEFAULT_PRODUCT_RETURN_SIZE = "DefaultProductReturnSize";
        private const string KEY_ELASTIC_SEARCH_AGGREGATIONS = "ElasticSearchAggregations";
        private const string KEY_ELASTIC_SEARCH_TERM_SEARCH_FIELDS = "ElasticSearchTermSearchFields";
        private const string KEY_ELASTIC_SEARCH_DIGIT_SEARCH_FIELDS = "ElasticSearchDigitSearchFields";
        private const string KEY_ELASTIC_SEARCH_BATCH_SIZE = "ElasticSearchBatchSize";
        private const string KEY_ELASTIC_SEARCH_URL = "ElasticSearchURL";
        private const string KEY_MAX_SORT_BY_PRICE_ITEM_COUNT = "MaxSortByPriceItemCount";
        private const string KEY_MF_ADDRESS = "MfAddress";
        private const string KEY_MF_PORT = "MfPort";
        private const string KEY_MF_TRANSACTIONID = "MfTrans";
        private const string KEY_MULTIDOCS_URL = "MultiDocsUrl";
        private const string KEY_RABBITMQ_EXCHANGE = "RabbitMQExchange";
        private const string KEY_RABBITMQ_ORDER_ADDRESS = "RabbitMQOrderServer";
        private const string KEY_RABBITMQ_ORDER_CONSUMEPASS = "RabbitMQOrderConsumerUserPassword";
        private const string KEY_RABBITMQ_ORDER_CONSUMEUSER = "RabbitMQOrderConsumerUserName";
        private const string KEY_RABBITMQ_ORDER_PUBLISHPASS = "RabbitMQOrderPublisherUserPassword";
        private const string KEY_RABBITMQ_ORDER_PUBLISHUSER = "RabbitMQOrderPublisherUserName";
        private const string KEY_RABBITMQ_ORDER_QUEUE = "RabbitMQOrderQueue";
        private const string KEY_RABBITMQ_ORDER_QUEUE_ERROR = "RabbitMQOrderErrorQueue";
        private const string KEY_RABBITMQ_ORDER_QUEUE_HISTORY = "RabbitMQOrderHistoryQueue";
        private const string KEY_RABBITMQ_ORDER_VHOST = "RabbitMQOrderVHost";

        private const string KEY_SITE_NAME = "CS_SiteName";

        #endregion

        #region methods
        private static List<string> GetCommaSeparatedValues(string val)
        {
            if (!String.IsNullOrEmpty(val))
                return (val.Split(new string[] { "," }, StringSplitOptions.None)).ToList();
            return new List<string>();
        }
        #endregion

        #region properties
        public static string ActiveDirectoryExternalDomain { 
            get { 
                return GetValue(KEY_AD_EXTERNAL_DOMAIN, string.Empty); 
            } 
        }

        public static string ActiveDirectoryExternalPassword
        {
            get
            {
                return GetValue(KEY_AD_EXTERNAL_PASSWORD, string.Empty);
            }
        }

        public static string ActiveDirectoryExternalRootNode
        {
            get
            {
                return GetValue(KEY_AD_EXTERNAL_ROOTNODE, string.Empty);
            }
        }

        public static string ActiveDirectoryExternalServerName
        {
            get
            {
                return GetValue(KEY_AD_EXTERNAL_SERVERNAME, string.Empty);
            }
        }

        public static string ActiveDirectoryExternalUserName
        {
            get
            {
                return GetValue(KEY_AD_EXTERNAL_USER, string.Empty);
            }
        }

        public static string ActiveDirectoryInternalDomain
        {
            get
            {
                return GetValue(KEY_AD_INTERNAL_DOMAIN, string.Empty);
            }
        }

        public static string ActiveDirectoryInternalPassword
        {
            get
            {
                return GetValue(KEY_AD_INTERNAL_PASSWORD, string.Empty);
            }
        }

        public static string ActiveDirectoryInternalRootNode
        {
            get
            {
                return GetValue(KEY_AD_INTERNAL_ROOTNODE, string.Empty);
            }
        }

        public static string ActiveDirectoryInternalServerName
        {
            get
            {
                return GetValue(KEY_AD_INTERNAL_SERVERNAME, string.Empty);
            }
        }

        public static string ActiveDirectoryInternalUserName
        {
            get
            {
                return GetValue(KEY_AD_INTERNAL_USER, string.Empty);
            }
        }

        public static int ActiveDirectoryInvalidAttempts { 
            get 
            {
                try
                {
                    return Convert.ToInt32(GetValue(KEY_AD_INVALIDATTEMPTS, "3"));
                }
                catch
                {
                    return 3;
                }
            } 
        }

        public static int ActiveDirectoryLockoutDuration { 
            get {
                try
                {
                    return Convert.ToInt32(GetValue(KEY_AD_LOCKOUTDURATION, "30"));

                }
                catch {
                    return 30;
                }
            } 
        }

        public static List<string> AllowedApiKeys
        {
            get
            {
                string val = GetValue(KEY_ALLOWED_API_KEYS, string.Empty);
                return GetCommaSeparatedValues(val);
            }
        }
        
        public static string AppDataConnectionString
        {
            get { return GetConnectionString(KEY_APPDATA_CONNECTIONSTRING); }
        }

        public static string ApplicationName
        {
            get
            {
                return GetValue(KEY_APP_NAME, DEFAULT_APPNAME);
            }
        }
        
        public static string BaseCatalog
        {
            get { return GetValue(KEY_BASE_CATALOG, string.Empty); }
        }

        public static string BrandAssetsUrl
        {
            get { return GetValue(KEY_BRAND_ASSETS_URL, string.Empty); }
        }

        public static string CategoryPrefixesToExclude
        {
            get { return GetValue(KEY_CATEGORY_PREFIXES, string.Empty); }
        }
        
        public static string CSSiteName
        {
            get { return GetValue(KEY_SITE_NAME, string.Empty); }
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
        
        public static string ElasticSearchAggregations
        {
            get { return GetValue(KEY_ELASTIC_SEARCH_AGGREGATIONS, string.Empty); }
        }

        public static int ElasticSearchBatchSize
        {
            get
            {
                string value = GetValue(KEY_ELASTIC_SEARCH_BATCH_SIZE, DEFAULT_ELASTIC_SEARCH_BATCH_SIZE);
                return ValueParsingUtil.ParseInt(value, DEFAULT_ELASTIC_SEARCH_BATCH_SIZE);
            }
        }
        
        public static List<string> ElasticSearchDigitSearchFields
        {
            get 
            { 
                string val = GetValue(KEY_ELASTIC_SEARCH_DIGIT_SEARCH_FIELDS, string.Empty);
                return GetCommaSeparatedValues(val);
            }
        }
        
        public static List<string> ElasticSearchTermSearchFields
        {
            get 
            {
                string val = GetValue(KEY_ELASTIC_SEARCH_TERM_SEARCH_FIELDS, string.Empty);
                return GetCommaSeparatedValues(val);
            }
        }

        public static string ElasticSearchURL
        {
            get { return GetValue(KEY_ELASTIC_SEARCH_URL, string.Empty); }
        }

        public static string MainframeIPAddress
        {
            get
            {
                return GetValue(KEY_MF_ADDRESS, "192.168.20.12");
            }
        }

        public static int MainframeListeningPort
        {
            get
            {
                return int.Parse(GetValue(KEY_MF_PORT, "0"));
            }
        }

        public static string MainframeOrderTransactionId
        {
            get
            {
                return GetValue(KEY_MF_TRANSACTIONID, string.Empty);
            }
        }

        public static int MaxSortByPriceItemCount
        {
            get
            {
                string value = GetValue(KEY_MAX_SORT_BY_PRICE_ITEM_COUNT, DEFAULT_MAX_SORT_BY_PRICE_ITEM_COUNT);
                return ValueParsingUtil.ParseInt(value, DEFAULT_ELASTIC_SEARCH_BATCH_SIZE);
            }
        }
        
        public static string MultiDocsUrl
        {
            get { return GetValue(KEY_MULTIDOCS_URL, string.Empty); }
        }

        public static string RabbitMQExchangeName {
            get {
                return GetValue(KEY_RABBITMQ_EXCHANGE, string.Empty);
            }
        }

        public static string RabbitMQOrderConsumerUserName
        {
            get
            {
                return GetValue(KEY_RABBITMQ_ORDER_CONSUMEUSER, string.Empty);
            }
        }

        public static string RabbitMQOrderConsumerUserPassword
        {
            get
            {
                return GetValue(KEY_RABBITMQ_ORDER_CONSUMEPASS, string.Empty);
            }
        }

        public static string RabbitMQOrderErrorQueue {
            get {
                return GetValue(KEY_RABBITMQ_ORDER_QUEUE_ERROR, string.Empty);
            }
        }

        public static string RabbitMQOrderHistoryQueue {
            get {
                return GetValue(KEY_RABBITMQ_ORDER_QUEUE_HISTORY, string.Empty);
            }
        }

        public static string RabbitMQOrderPublisherUserName
        {
            get
            {
                return GetValue(KEY_RABBITMQ_ORDER_PUBLISHUSER, string.Empty);
            }
        }

        public static string RabbitMQOrderPublisherUserPassword
        {
            get
            {
                return GetValue(KEY_RABBITMQ_ORDER_PUBLISHPASS, string.Empty);
            }
        }

        public static string RabbitMQOrderQueue {
            get {
                return GetValue(KEY_RABBITMQ_ORDER_QUEUE, string.Empty);
            }
        }

        public static string RabbitMQOrderServer
        {
            get
            {
                return GetValue(KEY_RABBITMQ_ORDER_ADDRESS, string.Empty);
            }
        }
        
        public static string RabbitMQOrderVHost
        {
            get
            {
                return GetValue(KEY_RABBITMQ_ORDER_VHOST, string.Empty);
            }
        }

        #endregion
    }
}