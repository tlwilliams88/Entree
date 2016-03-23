using KeithLink.Common.Core.Logging;
using KeithLink.Common.Impl.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Common.Impl.SettingsRepo
{
    public class DBAppSettingsRepositoryImpl
    {
        static DBAppSettingsRepositoryImpl _instance = new DBAppSettingsRepositoryImpl();
        private Dictionary<string, string> dict;
        private IEventLogRepository _log;

        public static DBAppSettingsRepositoryImpl getInstance()
        {
            if (_instance == null)
            {
                _instance = new DBAppSettingsRepositoryImpl();
            }
            return _instance;
        }
        private DBAppSettingsRepositoryImpl()
        {
            // Initialize
            string name = KeithLink.Common.Impl.Configuration.ApplicationName;
            _log = new EventLogRepositoryImpl(name);
            Init();
        }
        private void Init()
        {
            try
            {
                _log.WriteInformationLog("Initializing DBAppSettingsRepository");
                dict = new Dictionary<string, string>();
                using (var conn = new SqlConnection(KeithLink.Common.Impl.Configuration.AppDataConnectionString))
                {
                    using (var cmd = new SqlCommand("[Configuration].[ReadAppSettings]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader != null && reader.Read())
                            {
                                try
                                {
                                    dict.Add(Convert.ToString(reader["Key"]), Convert.ToString(reader["Value"]));
                                }
                                catch (Exception ex)
                                {
                                    _log.WriteErrorLog(" DBAppSettingsRepository[" + reader["Key"] + "]", ex);
                                }
                            }
                        }
                        _log.WriteInformationLog(" DBAppSettingsRepository, " + dict.Count + " settings");
                    }
                }
                _log.WriteInformationLog("Init DBAppSettingsRepository Complete");
            }
            catch (Exception ex)
            {
                _log.WriteErrorLog("Failed to initialize DBAppSettingsRepository", ex);
            } 
        }
        public static string GetValue(string key, string defval)
        {
            DBAppSettingsRepositoryImpl appsettings = DBAppSettingsRepositoryImpl.getInstance();
            return appsettings.GetInstanceValue(key, defval);
        }
        private string GetInstanceValue(string key, string defaultval)
        {
            try
            {
                string val = dict[key];
                //_log.WriteInformationLog(" DBAppSettingsRepository, dict[" + key + "] = " + val);
                if (val == null) return defaultval;
                return val;
            }catch(Exception ex)
            {
                // Apparently we expect some key's not to be found
                //_log.WriteErrorLog(" DBAppSettingsRepository[" + key + "]", ex);
                return null;
            }
        }
    }
}
// SQL to create
//create TABLE[Configuration].[AppSettings] (
//[Id] UNIQUEIDENTIFIER DEFAULT(newid()) NOT NULL,
//[Domain]  VARCHAR(50)    NOT NULL,
//[Comment]  VARCHAR(MAX)    NOT NULL,
//[SetKey]   VARCHAR(50)     NOT NULL,
//[Value]    VARCHAR(MAX)    NOT NULL,
//[Disabled] BIT DEFAULT((0)) NOT NULL,
//PRIMARY KEY CLUSTERED([Id] ASC)
//);
//CREATE PROCEDURE [Configuration].[ReadAppSettings] @domain nvarchar(50)
//AS
//BEGIN
//SET NOCOUNT ON;
//	SELECT 
//		SetKey, Value
//	FROM 
//		[Configuration].[AppSettings]
//	WHERE
//	    Domain = @domain and Disabled = 0
//END
// SQL to populate
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'PresentationUrl', 'http://localhost:8080/', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'AllowedApiKeys', 'android_dev_v1,iphone_dev_v1,web_dev_v1,', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'BrandAssetsUrl', 'http://devkeithlink.bekco.com/assets/brands', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'RequireHttps', 'false', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'RecentItemsToKeep', '10', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'ListItemDaysNew', '2', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'RunInternalServiceQueues', 'true', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'TokenDuration', '7', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'IsProduction', 'false', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings', 'OrderUpdateWatchPath', 'c:\test\entree\orderupdates\', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Site Settings: values are none, internal_only, all', 'EnableEtaForUsers', 'all', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Item History - Used to geenrate item averages over a period of weeks', 'ItemHistoryAverageWeeks', '8', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtDomain', 'benekeith', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtPass', 'D3leg@ti0n', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtRoot', 'ou=KEC_Development,dc=benekeith,dc=com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtServer', 'ldap-kec.benekeith.com', 1)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtServer', 'bekkecad1.benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtUser', 'KEC-SVC-LDAPAdmin', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtUserContainer', 'Users', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtAccessGroupKbitAdmin', 'Dev Kbit Admin', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtAccessGroupKbitCustomer', 'Dev Kbit Customer', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtAccessGroupPowerMenuCustomer', 'Dev PowerMenu Customer', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtRoleNameAccouting', 'Dev Accounting', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtRoleNameApprover', 'Dev Approver', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtRoleNameBuyer', 'Dev Buyer', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtRoleNameGuest', 'Dev Guest', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtRoleNameOwner', 'Dev Owner', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtAccessGroupKbitAdmin', 'Dev Kbit Admin', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtAccessGroupKbitCustomer', 'Dev Kbit Customer', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADExtAccessGroupPowerMenuCustomer', 'Dev PowerMenu Customer', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADIntDomain', 'bekco', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADIntPass', '@ud1tm3', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADIntRoleNameCorporateAdmin', 'CORP-LS-SYS-AC-Entree_DevAdmins', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADIntRoot', 'dc=bekco,dc=com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADIntServer', 'ldap-bek.benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADIntUser', 'corp-ssa-secaudit', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADBadPwdCount', '3', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADLockoutDuration', '3', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Active Directory Settings', 'ADGuestOU', '_bek_guest', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'WhiteList: must be lowercase', 'WhiteListedBekUsers', 'tcfox,jwames,pabrandt,mdjoiner,jdhughes,corp-ssa-entreadmin,dmderusha,bakillins,jmmills', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'WhiteList', 'WhiteListedBekUsersEnforced', 'false', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'ElasticSearchURL', 'http://localhost:9200', 1)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'ElasticSearchURL', 'http://corpkecqas2.bekco.com:9200', 1)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'ElasticSearchURL', 'http://shopdev.benekeith.com:9200', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'DefaultCategoryReturnSize', '2000', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'DefaultProductReturnSize', '500', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'MaxSortByPriceItemCount', '200', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'ElasticSearchAggregations', 'categories:categoryname_not_analyzed,brands:brand_description_not_analyzed,allergens:contains,dietary:diettype,itemspecs:itemspecification,nonstock:nonstock,mfrname:mfrname_not_analyzed', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'ElasticSearchTermSearchFields', 'name,description,categoryname,brand,mfrname', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'ElasticSearchDigitSearchFields', 'upc,gtin,itemnumber', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'ElasticSearchItemExcludeValues', 'D,!,W', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Elastic Search Settings', 'CategoryPrefixesToExclude', 'AA,ZZ,TW', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Commerce Server Settings', 'CS_SiteName', 'BEK_Commerce', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'CORS', 'CorsEnabledDomains', '*', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'CORS', 'CorsEnabledHeaders', '*', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'CORS', 'CorsEnabledMethods', '*', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Amazon Settings', 'AmazonSnsAccessKey', 'AKIAJ42DMK24ZMO56MYQ', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Amazon Settings', 'AmazonSnsSecretKey', 'TwrfjLm5y1G4xcUvYMAgn+/+kXDuyNUZIRD7qvA0', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Amazon Settings', 'AmazonSnsMobilePlatformAppArnIOS', 'arn:aws:sns:us-east-1:951996173818:app/APNS/BEK_APNS_TEST', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Amazon Settings', 'AmazonSnsMobilePlatformAppArnAndroid', 'arn:aws:sns:us-east-1:951996173818:app/GCM/BEK_GCM', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQAccessServer', 'corpkecdev1', 1)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderServer', 'corpkecdev1', 1)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQConfirmationServer', 'corpkecdev1', 1)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationServer', 'corpkecqas1', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQAccessServer', 'corpkecqas1', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderServer', 'corpkecqas1', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQConfirmationServer', 'corpkecqas1', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQAccessVHost', 'access_update', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderVHost', 'orders_createupdate', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQConfirmationVHost', 'orders_statusnotification', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationVHost', 'notifications_notify', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQAccessExchange', 'access_exchange', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderCreatedExchange', 'bek_commerce_orders_created', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderErrorExchange', 'bek_commerce_orders_error', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderReprocessExchange', 'bek_commerce_orders_reprocess', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderHistoryExchange', 'bek_commerce_orders_history', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQUpdateRequestExchange', 'bek_commerce_historyrequest', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderUpdateExchange', 'bek_commerce_orderupdates', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQSpecialOrderUpdateExchange', 'bek_commerce_specialorderupdates', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQConfirmationExchange', 'bek_commerce_confirmations', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQConfirmationErrorExchange', 'bek_commerce_confirmations_errors', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationExchange', 'bek_commerce_notifications', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationPublisherUserName', 'notifypub', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationPublisherUserPassword', 'notifypasspub', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQAccessConsumerUserName', 'accessclnt', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQAccessConsumerUserPassword', 'acspassclnt', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQAccessPublisherUserName', 'accesspub', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQAccessPublisherUserPassword', 'acspasspub', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderConsumerUserName', 'orderclnt', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderConsumerUserPassword', 'ordpassclnt', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderPublisherUserName', 'orderpub', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderPublisherUserPassword', 'ordpasspub', 0)	
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationPublisherUserName', 'notifypub', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationPublisherUserPassword', 'notifypasspub', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationConsumerUserName', 'notifyclnt', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationConsumerUserPassword', 'notifypassclnt', 0)	
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQAccessQueue', 'access_update_queue', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderQueue', 'orders_created_dev_jeremy', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQSpecialOrderUpdateQueue', 'specialorders_updates_dev', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderErrorQueue', 'orders_error', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderHistoryQueue', 'orders_history', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQUpdateRequestQueue', 'orders_historyrequest', 0)	
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderReprocessQueue', 'orders_reprocess', 0)	
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQOrderUpdateQueue', 'orders_updates_debug_default', 0)	
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationQueue', 'notifications_v1_dev', 0)	
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQNotificationQueueExternal', 'notifications_jason_external', 0)	
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQConfirmationQueue', 'confirmations_dev_josh', 0)	
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'RabbitMQ Settings', 'RabbitMQConfirmationErrorQueue', 'confirmations_error', 0)	
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'MultiDocs Settings', 'MultiDocsUrl', 'http://testmultidocs.bekco.com/', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'MultiDocs Settings', 'MultiDocsProxyUrl', 'http://testmultidocs.bekco.com/', 0)    
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'SmtpServer', 'corpampas1.bekco.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'SMTPHostName', 'smtp.credera.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'ServiceEmailAddress', 'noreply@credera.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'FailureEmailAddress', 'jwames@benekeith.com,mdjoiner@benekeith.com,bakillins@benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'FromEmailAddress', 'BekEntree@benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'FAQ_ContactEmail', 'faqinfo@benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'FAM_ContactEmail', 'amainfo@benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'FDF_ContactEmail', 'dfwinfo@benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'FHS_ContactEmail', 'fhsinfo@benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'FLR_ContactEmail', 'flrinfo@benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'FOK_ContactEmail', 'okinfo@benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'FSA_ContactEmail', 'fsainfo@benekeith.com', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Email Settings', 'ClientSettingsProvider.ServiceUri', '', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Mainframe Connection Settings', 'MfAddress', '192.168.20.12', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Mainframe Connection Settings', 'MfConfirmationPort', '4001', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Mainframe Connection Settings', 'MFOrderHistoryPort', '4002', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Mainframe Connection Settings', 'MfPort', '3011', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Mainframe Connection Settings', 'MfTrans', 'OT90', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Mainframe Connection Settings', 'MfTransHistory', 'OT96', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Mainframe Connection Settings', 'EntreeCollectorType', 'B', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Cache setttings', 'AddServerNameToHeaderResponse', 'true', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Cache setttings', 'CacheServerEndpoints', 'http://localhost:8080/api', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'imaging settings', 'ImagingServerUrl', 'http://bekimage.bekco.com:8080/integrationserver_727/', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'imaging settings', 'ImagingUserName', 'corp-ssa-imgentree', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'imaging settings', 'ImagingUserPassword', 'imagenow67', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'imaging settings', 'ImagingViewId', '200000054R_001QL95EY9CM', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'BillPay Settings: this setting must be in a 24 hour format', 'BillPayCutOffTime', '14:00:00', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'PowerMenu Settings', 'PowerMenuAdminUsername', 'pniadmin', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'PowerMenu Settings', 'PowerMenuAdminPassword', '9M9NC0N', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'PowerMenu Settings', 'PowerMenuWebServiceUrl', 'http://bekpmwsq1.bekco.com:8443/pmserviceendpoint.asmx', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'PowerMenu Settings', 'PowerMenuPermissionsUrl', 'http://bekpmwsq1.bekco.com:8443/main/Logon.aspx?Adminusername=pniadmin&amp;Adminpassword=9M9NC0N&amp;Username={0}&amp;path=USER', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'PowerMenu Settings', 'PowerMenuGroupSetupUrl', 'http://bekpmwsq1.bekco.com:8443/main/Logon.aspx?Adminusername=pniadmin&amp;Adminpassword=9M9NC0N&amp;path=CUSTOMER', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'PowerMenu Settings', 'PowerMenuLoginUrl', 'http://bekpmwsq1.bekco.com:8443/main/Logon.aspx?username={0}&amp;password={1}&amp;path=MAIN&amp;customerlist={2}&amp;order=true&amp;framed=false&amp;lang=ENG&amp;country=USA', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Content Management Settings', 'MarketingBranchItemCount', '3', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Content Management Settings', 'MarketingContentUrl', 'http://www.benekeith.com/api/app', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Content Management Settings', 'MarketingTotalItemCount', '6', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Misc Settings', 'WebNowUrl', 'http://invoice.benekeith.com/webnow/index.jsp?action=filter&amp;username=anonymous&amp;drawer={branch}AR501&amp;tab={customer}&amp;field4={invoice}', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Misc Settings', 'LogSystemPerformanceWithErrors', 'true', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Misc Settings', 'CacheServerEndpoints', 'http://localhost:1317/CacheService.svc', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Misc Settings', 'EntreeSiteURL', 'http://localhost:9000', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Queue Service Functions', 'CheckLostOrders', 'true', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'Queue Service Functions', 'CheckLostOrdersStatus', 'Submitted', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'UNFI Whitelisting', 'UNFIWhitelistDSRs', '', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'UNFI Whitelisting', 'UNFIWhitelistCustomers', '', 0)
//INSERT INTO[BEK_Commerce_AppData].[Configuration].[AppSettings] VALUES(newid(),'Entree', 'UNFI Whitelisting', 'UNFIWhitelistBEKUsers', 'tcfox,jwames,pabrandt,jmmills,mdjoiner,jdhughes,dmderusha,bakillins', 0)