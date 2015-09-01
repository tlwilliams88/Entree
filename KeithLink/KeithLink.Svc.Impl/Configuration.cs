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
        // General site settings
        private const string PRESENTATION_URL = "PresentationUrl";
        private const string DEFAULT_APPNAME = "Entree";
        private const string DEFAULT_CATEGORY_RETURN_SIZE = "2000";
        private const string DEFAULT_ELASTIC_SEARCH_BATCH_SIZE = "500";
        private const string DEFAULT_PRODUCT_RETURN_SIZE = "500";
        private const string DEFAULT_MAX_SORT_BY_PRICE_ITEM_COUNT = "200";
		private const string DEFAULT_RECENT_ITEMS_TO_KEEP = "5";
        private const string DEFAULT_ENABLE_ETA_FOR_USERS = "non";

        // Item History
        private const string KEY_ITEM_HISTORY_WEEKS = "ItemHistoryAverageWeeks";

        // Active Directory Constants
        private const string KEY_AD_EXTERNAL_ACCESSGROUP_KBITADMIN = "ADExtAccessGroupKbitAdmin";
        private const string KEY_AD_EXTERNAL_ACCESSGROUP_KBITCUSTOMER = "ADExtAccessGroupKbitCustomer";
        private const string KEY_AD_EXTERNAL_ACCESSGROUP_POWERMENUCUSTOMER = "ADExtAccessGroupPowerMenuCustomer";
        private const string KEY_AD_EXTERNAL_DOMAIN = "ADExtDomain";
        private const string KEY_AD_EXTERNAL_PASSWORD = "ADExtPass";
        private const string KEY_AD_EXTERNAL_ROOTNODE = "ADExtRoot";
        private const string KEY_AD_EXTERNAL_ROLENAME_ACCOUNTING = "ADExtRoleNameAccouting";
        private const string KEY_AD_EXTERNAL_ROLENAME_APPROVER = "ADExtRoleNameApprover";
        private const string KEY_AD_EXTERNAL_ROLENAME_BUYER = "ADExtRoleNameBuyer";
        private const string KEY_AD_EXTERNAL_ROLENAME_GUEST = "ADExtRoleNameGuest";
        private const string KEY_AD_EXTERNAL_ROLENAME_OWNER = "ADExtRoleNameOwner";
        private const string KEY_AD_EXTERNAL_SERVERNAME = "ADExtServer";
        private const string KEY_AD_EXTERNAL_USER = "ADExtUser";
        private const string KEY_AD_EXTERNAL_USERCONTAINER = "ADExtUserContainer";
        private const string KEY_AD_GUEST_CONTAINER = "ADGuestOU";
        private const string KEY_AD_INTERNAL_DOMAIN = "ADIntDomain";
        private const string KEY_AD_INTERNAL_PASSWORD = "ADIntPass";
        private const string KEY_AD_INTERNAL_ROOTNODE = "ADIntRoot";
        private const string KEY_AD_INTERNAL_SERVERNAME = "ADIntServer";
        private const string KEY_AD_INTERNAL_USER = "ADIntUser";
        private const string KEY_AD_INVALIDATTEMPTS = "ADBadPwdCount";
        private const string KEY_AD_LOCKOUTDURATION = "ADLockoutDuration";
        private const string KEY_WHITE_LISTED_BEK_USERS = "WhiteListedBekUsers";
        private const string KEY_WHITE_LISTED_BEK_USERS_ENFORCED = "WhiteListedBekUsersEnforced";

        // Elasticsearch / Commerce Server / Application specific
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
        private const string KEY_ELASTIC_SEARCH_ITEM_EXCLUDE_FIELDS = "ElasticSearchItemExcludeFields";
        private const string KEY_ELASTIC_SEARCH_ITEM_EXCLUDE_VALUES = "ElasticSearchItemExcludeValues";
        private const string KEY_ELASTIC_SEARCH_DIGIT_SEARCH_FIELDS = "ElasticSearchDigitSearchFields";
        private const string KEY_ELASTIC_SEARCH_BATCH_SIZE = "ElasticSearchBatchSize";
        private const string KEY_ELASTIC_SEARCH_URL = "ElasticSearchURL";
        private const string KEY_MAX_SORT_BY_PRICE_ITEM_COUNT = "MaxSortByPriceItemCount";
        private const string KEY_ENABLE_ETA_FOR_USERS = "EnableEtaForUsers";

        // Mainframe Keys
        private const string KEY_MF_ADDRESS = "MfAddress";
        private const string KEY_MF_CONFRIMATION_PORT = "MfConfirmationPort";
        private const string KEY_MF_ORDERHISTORY_PORT = "MFOrderHistoryPort";
        private const string KEY_MF_PORT = "MfPort";
        private const string KEY_MF_TRANS_ORDER = "MfTrans";
        private const string KEY_MF_TRANS_HISTORY = "MfTransHistory";

        // Rabbit MQ Constants
        private const string KEY_RABBITMQ_EXCHANGE_ACCESS = "RabbitMQAccessExchange";
        private const string KEY_RABBITMQ_EXCHANGE_CONFIRMATION = "RabbitMQConfirmationExchange";
        private const string KEY_RABBITMQ_EXCHANGE_CONFIRMATION_ERRORS = "RabbitMQConfirmationErrorExchange";
        private const string KEY_RABBITMQ_EXCHANGE_HOURLYUPDATES = "RabbitMQOrderUpdateExchange";
        private const string KEY_RABBITMQ_EXCHANGE_ORDER_CREATED = "RabbitMQOrderCreatedExchange";
        private const string KEY_RABBITMQ_EXCHANGE_ORDER_ERROR = "RabbitMQOrderErrorExchange";
        private const string KEY_RABBITMQ_EXCHANGE_ORDER_HISTORY = "RabbitMQOrderHistoryExchange";
        private const string KEY_RABBITMQ_EXCHANGE_ORDER_REPROCESS = "RabbitMQOrderReprocessExchange";
        private const string KEY_RABBITMQ_EXCHANGE_NOTIFICATION = "RabbitMQNotificationExchange";
        private const string KEY_RABBITMQ_EXCHANGE_ORDER_UPDATEREQUEST = "RabbitMQUpdateRequestExchange";
        private const string KEY_RABBITMQ_QUEUE_ACCESS = "RabbitMQAccessQueue";
        private const string KEY_RABBITMQ_QUEUE_CONFIRMATION = "RabbitMQConfirmationQueue";
        private const string KEY_RABBITMQ_QUEUE_CONFIRMATION_ERRORS = "RabbitMQConfirmationErrorQueue";
        private const string KEY_RABBITMQ_QUEUE_HOURLYUPDATES = "RabbitMQOrderUpdateQueue";
        private const string KEY_RABBITMQ_QUEUE_ORDER_CREATED = "RabbitMQOrderQueue";
        private const string KEY_RABBITMQ_QUEUE_ORDER_ERROR = "RabbitMQOrderErrorQueue";
        private const string KEY_RABBITMQ_QUEUE_ORDER_HISTORY = "RabbitMQOrderHistoryQueue";
        private const string KEY_RABBITMQ_QUEUE_ORDER_REPROCESS = "RabbitMQOrderReprocessQueue";
        private const string KEY_RABBITMQ_QUEUE_NOTIFICATION = "RabbitMQNotificationQueue";
        private const string KEY_RABBITMQ_QUEUE_NOTIFICATION_EXTERNAL = "RabbitMQNotificationQueueExternal";
        private const string KEY_RABBITMQ_QUEUE_ORDER_UPDATEREQUEST = "RabbitMQUpdateRequestQueue";
        private const string KEY_RABBITMQ_SERVER_ACCESS = "RabbitMQAccessServer";
        private const string KEY_RABBITMQ_SERVER_ORDER = "RabbitMQOrderServer";
        private const string KEY_RABBITMQ_SERVER_CONFIRMATION = "RabbitMQConfirmationServer";
        private const string KEY_RABBITMQ_SERVER_NOTIFICATION = "RabbitMQNotificationServer";
        private const string KEY_RABBITMQ_USER_ACCESS_CONSUMEPASS = "RabbitMQAccessConsumerUserPassword";
        private const string KEY_RABBITMQ_USER_ACCESS_CONSUMEUSER = "RabbitMQAccessConsumerUserName";
        private const string KEY_RABBITMQ_USER_ACCESS_PUBLISHPASS = "RabbitMQAccessPublisherUserPassword";
        private const string KEY_RABBITMQ_USER_ACCESS_PUBLISHUSER = "RabbitMQAccessPublisherUserName";
        private const string KEY_RABBITMQ_USER_ORDER_CONSUMEPASS = "RabbitMQOrderConsumerUserPassword";
        private const string KEY_RABBITMQ_USER_ORDER_CONSUMEUSER = "RabbitMQOrderConsumerUserName";
        private const string KEY_RABBITMQ_USER_ORDER_PUBLISHPASS = "RabbitMQOrderPublisherUserPassword";
        private const string KEY_RABBITMQ_USER_ORDER_PUBLISHUSER = "RabbitMQOrderPublisherUserName";
        private const string KEY_RABBITMQ_USER_NOTIFICATION_CONSUMEPASS = "RabbitMQNotificationConsumerUserPassword";
        private const string KEY_RABBITMQ_USER_NOTIFICATION_CONSUMEUSER = "RabbitMQNotificationConsumerUserName";
        private const string KEY_RABBITMQ_USER_NOTIFICATION_PUBLISHPASS = "RabbitMQNotificationPublisherUserPassword";
        private const string KEY_RABBITMQ_USER_NOTIFICATION_PUBLISHUSER = "RabbitMQNotificationPublisherUserName";
        private const string KEY_RABBITMQ_VHOST_ACCESS = "RabbitMQAccessVHost";
        private const string KEY_RABBITMQ_VHOST_CONFIRMATION = "RabbitMQConfirmationVHost";
        private const string KEY_RABBITMQ_VHOST_NOTIFICATION = "RabbitMQNotificationVHost";
        private const string KEY_RABBITMQ_VHOST_ORDER = "RabbitMQOrderVHost";
        private const string KEY_RUN_INTERNAL_SERVICE_QUEUES = "RunInternalServiceQueues";
        private const string KEY_AMAZON_SNS_ACCESS_KEY = "AmazonSnsAccessKey";
        private const string KEY_AMAZON_SNS_SECRET_KEY = "AmazonSnsSecretKey";
        private const string KEY_AMAZON_SNS_MOBILE_PLATFORM_APP_ARN_IOS = "AmazonSnsMobilePlatformAppArnIOS";
        private const string KEY_AMAZON_SNS_MOBILE_PLATFORM_APP_ARN_ANDROID = "AmazonSnsMobilePlatformAppArnAndroid";

        // Misc
        private const string KEY_APP_NAME = "AppName";
        private const string KEY_DURATION_TOKEN = "TokenDuration";
        private const string KEY_MULTIDOCS_URL = "MultiDocsUrl";
        private const string KEY_MULTIDOCS_PROXY_URL = "MultiDocsProxyUrl";
		private const string KEY_REQUIRE_HTTPS = "RequireHttps";
		private const string KEY_RECENT_ITEMS_TO_KEEP = "RecentItemsToKeep";
        private const string KEY_SITE_NAME = "CS_SiteName";
        private const string KEY_PATH_ORDERUPDATES = "OrderUpdateWatchPath";
        private const string LIST_ITEM_DAYS_NEW = "ListItemDaysNew";
        private const string KEY_URL_WEBNOW = "WebNowUrl";
        private const string KEY_ENVIRONMENT_DEMO = "IsDemoEnvironment";
		private const string KEY_ENTREE_SITE_URL = "EntreeSiteURL";

		//Email
		private const string KEY_SMTP_FROMADDRESS = "FromEmailAddress";
		private const string KEY_SMTP_SERVERNAME = "SmtpServer";
		private const string KEY_SMTP_SEND_PORT = "SMTPSendPort";
		private const string DEFAULT_SMTP_SEND_PORT = "25";
		private const string KEY_SMTP_USERNAME = "SMTPUsername";
		private const string KEY_SMTP_PASSWORD = "SMTPPassword";

		private const string KEY_CONTACT_EMAIL_FORMAT = "{0}_ContactEmail";
		private const string KEY_ADD_SERVERNAME_TO_HEADER = "AddServerNameToHeaderResponse";
		private const string KEY_CACHE_SERVERSENDPOINTS = "CacheServerEndpoints";

        //Single Sign On
        private const string KEY_SSO_KBIT_CONNECTIONSTRING = "KbitCustomer";

        // Imaging (ImageNow/WebNow)
        private const string KEY_IMG_PASSWORD = "ImagingUserPassword";
        private const string KEY_IMG_SERVER = "ImagingServerUrl";
        private const string KEY_IMG_USER = "ImagingUserName";
        private const string KEY_IMG_VIEWID = "ImagingViewId";

        //PowerMenu
        private const string KEY_POWERMENU_ADMIN_USERNAME = "PowerMenuAdminUsername";
        private const string KEY_POWERMENU_ADMIN_PASSWORD = "PowerMenuAdminPassword";
        private const string KEY_POWERMENU_WEBSERVICE_URL = "PowerMenuWebServiceUrl";
        private const string KEY_POWERMENU_PERMISSIONS_URL = "PowerMenuPermissionsUrl";
        private const string KEY_POWERMENU_LOGIN_URL = "PowerMenuLoginUrl";
        private const string KEY_POWERMENU_GROUP_SETUP_URL = "PowerMenuGroupSetupUrl";

        // Content Management
        private const string KEY_MARKETINGCONTENT_BRANCHITEMCOUNT = "MarketingBranchItemCount";
        private const string KEY_MARKETINGCONTENT_TOTALITEMCOUNT = "MarketingTotalItemCount";
        private const string KEY_MARKETINGCONTENT_URL = "MarketingContentUrl";
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
        public static string AccessGroupKbitAdmin {
            get {
                return GetValue(KEY_AD_EXTERNAL_ACCESSGROUP_KBITADMIN, string.Empty);
            }
        }

        public static string AccessGroupKbitCustomer {
            get {
                return GetValue(KEY_AD_EXTERNAL_ACCESSGROUP_KBITCUSTOMER, string.Empty);
            }
        }

        public static string AccessGroupPowerMenuCustomer {
            get {
                return GetValue( KEY_AD_EXTERNAL_ACCESSGROUP_POWERMENUCUSTOMER, string.Empty );
            }
        }

        public static string ActiveDirectoryExternalDomain { 
            get { 
                return GetValue(KEY_AD_EXTERNAL_DOMAIN, string.Empty); 
            } 
        }

        public static string ActiveDirectoryExternalDomainUserName {
            get {
                return string.Format("{0}\\{1}", ActiveDirectoryExternalDomain, ActiveDirectoryExternalUserName);
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

        public static string ActiveDirectoryExternalUserContainer {
            get {
                return GetValue(KEY_AD_EXTERNAL_USERCONTAINER, "Users");
            }
        }

        public static string ActiveDirectoryGuestContainer {
            get { return GetValue(KEY_AD_GUEST_CONTAINER, string.Empty); }
        }

        public static string ActiveDirectoryInternalDomain
        {
            get
            {
                return GetValue(KEY_AD_INTERNAL_DOMAIN, string.Empty);
            }
        }

        public static string ActiveDirectoryInternalDomainUserName {
            get {
                return string.Format("{0}\\{1}", ActiveDirectoryInternalDomain, ActiveDirectoryInternalUserName);
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

        public static bool AddServerNameToHeader {
            get {
                var value = GetValue(KEY_ADD_SERVERNAME_TO_HEADER, "false");
                return ValueParsingUtil.ParseBool(value, "false");
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

        public static string AmazonSnsAccessKey {
            get {
                return GetValue(KEY_AMAZON_SNS_ACCESS_KEY, string.Empty);
            }
        }

        public static string AmazonSnsMobilePlatformAppArnAndroid {
            get {
                return GetValue(KEY_AMAZON_SNS_MOBILE_PLATFORM_APP_ARN_ANDROID, string.Empty);
            }
        }
        
        public static string AmazonSnsMobilePlatformAppArnIOS {
            get {
                return GetValue(KEY_AMAZON_SNS_MOBILE_PLATFORM_APP_ARN_IOS, string.Empty);
            }
        }

        public static string AmazonSnsSecretKey {
            get {
                return GetValue(KEY_AMAZON_SNS_SECRET_KEY, string.Empty);
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

		public static string BranchContactEmail(string branchId)
		{
			return GetValue(string.Format(KEY_CONTACT_EMAIL_FORMAT, branchId.ToUpper()), string.Empty);

		}

        public static string BrandAssetsUrl
        {
            get { return GetValue(KEY_BRAND_ASSETS_URL, string.Empty); }
        }

        public static List<string> CacheServersEndpoints {
            get {
                string value = GetValue(KEY_CACHE_SERVERSENDPOINTS, string.Empty);
                return GetCommaSeparatedValues(value);
            }
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

        public static string EnableEtaForUsers
        {
            get { return GetValue(KEY_ENABLE_ETA_FOR_USERS, DEFAULT_ENABLE_ETA_FOR_USERS); }
        }

        public static List<string> ElasticSearchDigitSearchFields
        {
            get 
            { 
                string val = GetValue(KEY_ELASTIC_SEARCH_DIGIT_SEARCH_FIELDS, string.Empty);
                return GetCommaSeparatedValues(val);
            }
        }

        public static string ElasticSearchItemExcludeValues {
            get { return GetValue(KEY_ELASTIC_SEARCH_ITEM_EXCLUDE_VALUES, string.Empty); }
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

        public static string EntreeSiteURL {
            get {
                return GetValue(KEY_ENTREE_SITE_URL, string.Empty);
            }
        }

        public static string ImagingServerUrl {
            get {
                return GetValue(KEY_IMG_SERVER, string.Empty);
            }
        }

        public static string ImagingUserName {
            get {
                return GetValue(KEY_IMG_USER, string.Empty);
            }
        }

        public static string ImagingUserPassword {
            get {
                return GetValue(KEY_IMG_PASSWORD, string.Empty);
            }
        }

        public static string ImagingViewId {
            get {
                return GetValue(KEY_IMG_VIEWID, String.Empty);
            }
        }

        public static bool IsDemoEnvironment {
            get {
                var value = GetValue(KEY_ENVIRONMENT_DEMO, "false");
                return ValueParsingUtil.ParseBool(value, "false");
            }
        }

        public static int ItemHistoryAverageWeeks {
            get {
                return ValueParsingUtil.ParseInt(GetValue( KEY_ITEM_HISTORY_WEEKS, String.Empty ), String.Empty);
            }
        }
        public static string KbitConnectionString {
            get {
                return GetConnectionString(KEY_SSO_KBIT_CONNECTIONSTRING);
            }
        }

        public static double ListItemsDaysNew {
            get {
                string value = GetValue(LIST_ITEM_DAYS_NEW, String.Empty);
                return ValueParsingUtil.ParseDouble(value, String.Empty);
            }
        }

        public static int LoginTokenDuration {
            get {
                return int.Parse(GetValue(KEY_DURATION_TOKEN, "1"));
            }
        }

        public static string MainframeIPAddress
        {
            get
            {
                return GetValue(KEY_MF_ADDRESS, "192.168.20.12");
            }
        }

        public static int MainframeConfirmationListeningPort
        {
            get
            {
                return int.Parse(GetValue(KEY_MF_CONFRIMATION_PORT, "4001"));
            }
        }

        public static int MainframOrderHistoryListeningPort {
            get {
                return int.Parse(GetValue(KEY_MF_ORDERHISTORY_PORT, "4002"));
            }
        }

        public static int MainframeListeningPort
        {
            get
            {
                return int.Parse(GetValue(KEY_MF_PORT, "0"));
            }
        }

        public static string MainframeHistoryTransactionId {
            get {
                return GetValue(KEY_MF_TRANS_HISTORY, string.Empty);
            }
        }

        public static string MainframeOrderTransactionId
        {
            get
            {
                return GetValue(KEY_MF_TRANS_ORDER, string.Empty);
            }
        }

        public static int MarketingContentBranchItemCount {
            get {
                return int.Parse(GetValue(KEY_MARKETINGCONTENT_BRANCHITEMCOUNT, "0"));
            }
        }

        public static int MarketingContentTotalItemCount {
            get {
                return int.Parse(GetValue(KEY_MARKETINGCONTENT_TOTALITEMCOUNT, "0"));
            }
        }

        public static string MarketingContentUrl {
            get {
                return GetValue(KEY_MARKETINGCONTENT_URL, string.Empty);
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
            get 
            { 
                string configValue = GetValue(KEY_MULTIDOCS_URL, string.Empty);
                if (!String.IsNullOrEmpty(configValue) && !configValue.EndsWith("/"))
                    configValue = configValue + "/";
                return configValue;
            }
        }

        public static string MultiDocsProxyUrl
        {
            get 
            { 
                string configValue = GetValue(KEY_MULTIDOCS_PROXY_URL, string.Empty);
                if (!String.IsNullOrEmpty(configValue) && !configValue.EndsWith("/"))
                    configValue = configValue + "/";
                return configValue;
            }
        }

        public static string OrderUpdateWatchPath {
            get{ return GetValue(KEY_PATH_ORDERUPDATES, string.Empty); }
        }

        public static string PowerMenuAdminUsername {
            get { return GetValue( KEY_POWERMENU_ADMIN_USERNAME, string.Empty ); }
        }

        public static string PowerMenuAdminPassword {
            get { return GetValue( KEY_POWERMENU_ADMIN_PASSWORD, string.Empty ); }
        }

        public static string PowerMenuWebServiceUrl {
            get { return GetValue( KEY_POWERMENU_WEBSERVICE_URL, string.Empty ); }
        }

        public static string PowerMenuPermissionsUrl {
            get { return GetValue( KEY_POWERMENU_PERMISSIONS_URL, string.Empty ); }
        }
        
        public static string PowerMenuLoginUrl {
            get { return GetValue( KEY_POWERMENU_LOGIN_URL, string.Empty ); }
        }

        public static string PowerMenuGroupSetupUrl {
            get { return GetValue( KEY_POWERMENU_GROUP_SETUP_URL, string.Empty ); }
        }

        public static string PresentationUrl {
            get { return GetValue( PRESENTATION_URL, string.Empty ); }
        }

        public static string RabbitMQExchangeAccess {
            get {
                return GetValue(KEY_RABBITMQ_EXCHANGE_ACCESS, string.Empty);
            }
        }

        public static string RabbitMQExchangeConfirmation {
            get {
                return GetValue(KEY_RABBITMQ_EXCHANGE_CONFIRMATION, string.Empty);
            }
        }

        public static string RabbitMQExchangeConfirmationErrors {
            get {
                return GetValue(KEY_RABBITMQ_EXCHANGE_CONFIRMATION_ERRORS, string.Empty);
            }
        }
        
        public static string RabbitMQExchangeHourlyUpdates {
            get { return GetValue(KEY_RABBITMQ_EXCHANGE_HOURLYUPDATES, string.Empty); }
        }

        public static string RabbitMQExchangeOrdersCreated {
            get {
                return GetValue(KEY_RABBITMQ_EXCHANGE_ORDER_CREATED, string.Empty);
            }
        }

        public static string RabbitMQExchangeOrdersError {
            get {
                return GetValue(KEY_RABBITMQ_EXCHANGE_ORDER_ERROR, string.Empty);
            }
        }

        public static string RabbitMQExchangeOrdersHistory {
            get {
                return GetValue(KEY_RABBITMQ_EXCHANGE_ORDER_HISTORY, string.Empty);
            }
        }

        public static string RabbitMQExchangeNotification
        {
            get
            {
                return GetValue(KEY_RABBITMQ_EXCHANGE_NOTIFICATION, string.Empty);
            }
        }

        public static string RabbitMQExchangeOrdersReprocess {
            get {
                return GetValue(KEY_RABBITMQ_EXCHANGE_ORDER_REPROCESS, string.Empty);
            }
        }

        public static string RabbitMQExchangeOrderUpdateRequests {
            get {
                return GetValue(KEY_RABBITMQ_EXCHANGE_ORDER_UPDATEREQUEST, string.Empty);
            }
        }

        public static string RabbitMQQueueAccess {
            get {
                return GetValue(KEY_RABBITMQ_QUEUE_ACCESS, string.Empty);
            }
        }

        public static string RabbitMQQueueConfirmation {
            get {
                return GetValue(KEY_RABBITMQ_QUEUE_CONFIRMATION, string.Empty);
            }
        }

        public static string RabbitMQQueueConfirmationErrors {
            get {
                return GetValue(KEY_RABBITMQ_QUEUE_CONFIRMATION_ERRORS, string.Empty);
            }
        }

        public static string RabbitMQQueueHourlyUpdates {
            get { return GetValue(KEY_RABBITMQ_QUEUE_HOURLYUPDATES, string.Empty); }
        }

        public static string RabbitMQQueueOrderCreated {
            get {
                return GetValue(KEY_RABBITMQ_QUEUE_ORDER_CREATED, string.Empty);
            }
        }

        public static string RabbitMQQueueOrderError {
            get {
                return GetValue(KEY_RABBITMQ_QUEUE_ORDER_ERROR, string.Empty);
            }
        }

        public static string RabbitMQQueueOrderHistory {
            get {
                return GetValue(KEY_RABBITMQ_QUEUE_ORDER_HISTORY, string.Empty);
            }
        }

        public static string RabbitMQQueueOrderReprocess {
            get {
                return GetValue(KEY_RABBITMQ_QUEUE_ORDER_REPROCESS, string.Empty);
            }
        }

        public static string RabbitMQQueueNotification
        {
            get
            {
                return GetValue(KEY_RABBITMQ_QUEUE_NOTIFICATION, string.Empty);
            }
        }

        public static string RabbitMQQueueNotificationExternal
        {
            get
            {
                return GetValue(KEY_RABBITMQ_QUEUE_NOTIFICATION_EXTERNAL, string.Empty);
            }
        }

        public static string RabbitMQQueueOrderUpdateRequest {
            get {
                return GetValue(KEY_RABBITMQ_QUEUE_ORDER_UPDATEREQUEST, string.Empty);
            }
        }

        public static string RabbitMQAccessUserNameConsumer {
            get {
                return GetValue(KEY_RABBITMQ_USER_ACCESS_CONSUMEUSER, string.Empty);
            }
        }

        public static string RabbitMQAccessUserNamePublisher {
            get {
                return GetValue(KEY_RABBITMQ_USER_ACCESS_PUBLISHUSER, string.Empty);
            }
        }

        public static string RabbitMQAccessUserPasswordConsumer {
            get {
                return GetValue(KEY_RABBITMQ_USER_ACCESS_CONSUMEPASS, string.Empty);
            }
        }

        public static string RabbitMQAccessUserPasswordPublisher {
            get {
                return GetValue(KEY_RABBITMQ_USER_ACCESS_PUBLISHPASS, string.Empty);
            }
        }

        public static string RabbitMQUserNameConsumer {
            get {
                return GetValue(KEY_RABBITMQ_USER_ORDER_CONSUMEUSER, string.Empty);
            }
        }

        public static string RabbitMQUserNamePublisher
        {
            get
            {
                return GetValue(KEY_RABBITMQ_USER_ORDER_PUBLISHUSER, string.Empty);
            }
        }

        public static string RabbitMQUserPasswordConsumer {
            get {
                return GetValue(KEY_RABBITMQ_USER_ORDER_CONSUMEPASS, string.Empty);
            }
        }
        
        public static string RabbitMQUserPasswordPublisher
        {
            get
            {
                return GetValue(KEY_RABBITMQ_USER_ORDER_PUBLISHPASS, string.Empty);
            }
        }

        public static string RabbitMQNotificationUserNameConsumer
        {
            get
            {
                return GetValue(KEY_RABBITMQ_USER_NOTIFICATION_CONSUMEUSER, string.Empty);
            }
        }

        public static string RabbitMQNotificationUserNamePublisher
        {
            get
            {
                return GetValue(KEY_RABBITMQ_USER_NOTIFICATION_PUBLISHUSER, string.Empty);
            }
        }

        public static string RabbitMQNotificationUserPasswordConsumer
        {
            get
            {
                return GetValue(KEY_RABBITMQ_USER_NOTIFICATION_CONSUMEPASS, string.Empty);
            }
        }

        public static string RabbitMQNotificationUserPasswordPublisher
        {
            get
            {
                return GetValue(KEY_RABBITMQ_USER_NOTIFICATION_PUBLISHPASS, string.Empty);
            }
        }

        public static string RabbitMQAccessServer {
            get {
                return GetValue(KEY_RABBITMQ_SERVER_ACCESS, string.Empty);
            }
        }

        public static string RabbitMQConfirmationServer
        {
            get
            {
                return GetValue(KEY_RABBITMQ_SERVER_CONFIRMATION, string.Empty);
            }
        }

        public static string RabbitMQOrderServer
        {
            get
            {
                return GetValue(KEY_RABBITMQ_SERVER_ORDER, string.Empty);
            }
        }

        public static string RabbitMQNotificationServer
        {
            get
            {
                return GetValue(KEY_RABBITMQ_SERVER_NOTIFICATION, string.Empty);
            }
        }

        public static string RabbitMQVHostAccess
        {
            get
            {
                return GetValue(KEY_RABBITMQ_VHOST_ACCESS, string.Empty);
            }
        }

        public static string RabbitMQVHostConfirmation {
            get {
                return GetValue(KEY_RABBITMQ_VHOST_CONFIRMATION, string.Empty);
            }
        }

        public static string RabbitMQVHostNotification {
            get {
                return GetValue(KEY_RABBITMQ_VHOST_NOTIFICATION, string.Empty);
            }
        }

        public static string RabbitMQVHostOrder
        {
            get
            {
                return GetValue(KEY_RABBITMQ_VHOST_ORDER, string.Empty);
            }
        }

        public static int RecentItemsToKeep {
            get {
                string value = GetValue(KEY_RECENT_ITEMS_TO_KEEP, DEFAULT_RECENT_ITEMS_TO_KEEP);
                return ValueParsingUtil.ParseInt(value, DEFAULT_RECENT_ITEMS_TO_KEEP);
            }
        }

        public static bool RequireHttps {
            get {
                var value = GetValue(KEY_REQUIRE_HTTPS, "false");
                return ValueParsingUtil.ParseBool(value, "false");
            }
        }
		
        public static string RoleNameAccounting {
            get {
                return GetValue(KEY_AD_EXTERNAL_ROLENAME_ACCOUNTING, string.Empty);
            }
        }

        public static string RoleNameApprover {
            get {
                return GetValue(KEY_AD_EXTERNAL_ROLENAME_APPROVER, string.Empty);
            }
        }

        public static string RoleNameBuyer {
            get {
                return GetValue(KEY_AD_EXTERNAL_ROLENAME_BUYER, string.Empty);
            }
        }

        public static string RoleNameGuest {
            get {
                return GetValue(KEY_AD_EXTERNAL_ROLENAME_GUEST, string.Empty);
            }
        }

        public static string RoleNameOwner {
            get {
                return GetValue(KEY_AD_EXTERNAL_ROLENAME_OWNER, string.Empty);
            }
        }

        public static bool RunInternalServiceQueues
        {
            get
            {
                return bool.Parse(GetValue(KEY_RUN_INTERNAL_SERVICE_QUEUES, "true"));
            }
        }
		
        public static string ServiceEmailAddress
		{
			get { return GetValue(KEY_SMTP_FROMADDRESS, null); }
		}

        public static string SMTPHostName
		{
			get { return GetValue(KEY_SMTP_SERVERNAME, null); }
		}

		public static int SMTPSendPort
		{
			get
			{
				string value = GetValue(KEY_SMTP_SEND_PORT, DEFAULT_SMTP_SEND_PORT);
				return ValueParsingUtil.ParseInt(value, DEFAULT_SMTP_SEND_PORT);
			}
		}

		public static string SMTPUsername
		{
			get { return GetValue(KEY_SMTP_USERNAME, null); }
		}

		public static string SMTPPassword
		{
			get { return GetValue(KEY_SMTP_PASSWORD, null); }
		}

        public static string WebNowUrl {
            get {
                return GetValue(KEY_URL_WEBNOW, string.Empty);
            }
        }

        public static List<string> WhiteListedBekUsers {
            get {
                string val = GetValue(KEY_WHITE_LISTED_BEK_USERS, string.Empty);
                return GetCommaSeparatedValues(val);
            }
        }

        public static bool WhiteListedBekUsersEnforced {
            get {
                try {
                    return Convert.ToBoolean(GetValue(KEY_WHITE_LISTED_BEK_USERS_ENFORCED, "true"));
                } catch {
                    return true;
                }
            }
        }
        #endregion
    }
}