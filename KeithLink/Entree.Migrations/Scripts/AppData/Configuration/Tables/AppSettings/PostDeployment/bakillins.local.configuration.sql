declare @AppSettings as TABLE 
( 
    [Key]      VARCHAR (50)  NOT NULL,
    [Value]    VARCHAR (MAX) NOT NULL,
    [Comment]  VARCHAR (MAX) NOT NULL,
    [Disabled] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC)
)

/* AD Settings */
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADBadPwdCount', '3', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('AddServerNameToHeaderResponse', 'true', 'Cache setttings', 0)

/* AD External Groups */
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtAccessGroupKbitAdmin', 'Dev Kbit Admin', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtAccessGroupKbitCustomer', 'Dev Kbit Customer', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtAccessGroupPowerMenuCustomer', 'Dev PowerMenu Customer', 'Active Directory Settings', 0)

/* AD Server Settings */
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtDomain', 'benekeith', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtPass', 'D3leg@ti0n', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtRoot', 'ou=KEC_Development,dc=benekeith,dc=com', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtServer', 'bekkecad1.benekeith.com', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtUser', 'KEC-SVC-LDAPAdmin', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtUserContainer', 'Users', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADGuestOU', '_bek_guest', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADIntDomain', 'bekco', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADIntPass', '@ud1tm3', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADIntRoleNameCorporateAdmin', 'CORP-LS-SYS-AC-Entree_DevAdmins', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADIntRoot', 'dc=bekco,dc=com', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADIntServer', 'ldap-bek.benekeith.com', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADIntUser', 'corp-ssa-secaudit', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADLockoutDuration', '3', 'Active Directory Settings', 0)

/* AD Role Groups */
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtRoleNameAccouting', 'Dev Accounting', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtRoleNameApprover', 'Dev Approver', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtRoleNameBuyer', 'Dev Buyer', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtRoleNameGuest', 'Dev Guest', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtRoleNameOwner', 'Dev Owner', 'Active Directory Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ADExtPermissionViewInvoices', 'Dev ViewInvoices', 'Active Directory Settings', 0)

INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CartOrOrder2ListIdPurgeDays', '-7', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('AllowedApiKeys', 'android_dev_v1,iphone_dev_v1,web_dev_v1,', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('AmazonSnsAccessKey', 'AKIAJ42DMK24ZMO56MYQ', 'Amazon Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('AmazonSnsMobilePlatformAppArnAndroid', 'arn:aws:sns:us-east-1:951996173818:app/GCM/BEK_GCM', 'Amazon Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('AmazonSnsMobilePlatformAppArnIOS', 'arn:aws:sns:us-east-1:951996173818:app/APNS/BEK_APNS_TEST', 'Amazon Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('AmazonSnsSecretKey', 'TwrfjLm5y1G4xcUvYMAgn+/+kXDuyNUZIRD7qvA0', 'Amazon Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('AppName', 'Entree', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('BillPayCutOffTime', '14:00:00', 'BillPay Settings: this setting must be in a 24 hour format', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('BrandAssetsUrl', 'http://devkeithlink.bekco.com/assets/brands', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CacheServerEndpoints', 'http://corpmisdev2h.bekco.com:60606', 'Misc Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CategoryPrefixesToExclude', 'AA,ZZ,TW', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CheckForAppSettingsChangeDelayMinutes', '3', 'DB Has Changed', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ClientSettingsProvider.ServiceUri', '', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CorsEnabledDomains', '*', 'CORS', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CorsEnabledHeaders', '*', 'CORS', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CorsEnabledMethods', '*', 'CORS', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CS_SiteName', 'BEK_Commerce', 'Commerce Server Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('DBChangeValue', '0.34754812128262', 'DB Has Changed (change this to make clients reset)', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('DefaultCategoryReturnSize', '2000', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('DefaultProductReturnSize', '500', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ElasticSearchAggregations', 'parentcategories:parentcategoryname_not_analyzed,categories:categoryname_not_analyzed,brands:brand_description_not_analyzed,allergens:nutritional.allergen.contains,dietary:nutritional.diet.diettype,itemspecs:itemspecification,nonstock:nonstock,mfrname:mfrname_not_analyzed', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ElasticSearchBatchSize', '1000', 'Process ES Items in batch', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ElasticSearchDigitSearchFields', 'upc.ngram,gtin.ngram,itemnumber.ngram^5,mfrnumber.ngram^3', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ElasticSearchItemExcludeValues', 'D,!,W', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ElasticSearchTermSearchFields', 'name^5,name.english^4,description.english,categoryname,brand,mfrname,mfrnumber,name_ngram_analyzed', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ElasticSearchURL', 'http://qaes.benekeith.com:9200', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('EnableEtaForUsers', 'all', 'Site Settings: values are none, internal_only, all', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('EntreeCollectorType', 'B', 'Mainframe Connection Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('EntreeSiteURL', 'http://corpmisdev2a.bekco.com:9000', 'Misc Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('FailureEmailAddress', 'bakillins@benekeith.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('FAM_ContactEmail', 'amainfo@benekeith.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('FAQ_ContactEmail', 'faqinfo@benekeith.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('FDF_ContactEmail', 'dfwinfo@benekeith.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('FHS_ContactEmail', 'fhsinfo@benekeith.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('FLR_ContactEmail', 'flrinfo@benekeith.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('FOK_ContactEmail', 'okinfo@benekeith.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('FromEmailAddress', 'BekEntree@benekeith.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('FSA_ContactEmail', 'fsainfo@benekeith.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ImagingServerUrl', 'http://bekimage.bekco.com:8080/integrationserver_937/', 'imaging settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ImagingUserName', 'corp-ssa-imgentree', 'imaging settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ImagingUserPassword', 'imagenow67', 'imaging settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ImagingViewId', '200000054R_001QL95EY9CM', 'imaging settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('IsProduction', 'false', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ItemHistoryAverageWeeks', '8', 'Item History - Used to geenrate item averages over a period of weeks', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ListItemDaysNew', '2', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('LogSystemPerformanceWithErrors', 'true', 'Misc Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MarketingBranchItemCount', '3', 'Content Management Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MarketingContentUrl', 'http://www.benekeith.com/api/app', 'Content Management Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MarketingTotalItemCount', '6', 'Content Management Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MaxSortByPriceItemCount', '200', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MfAddress', '192.168.20.12', 'Mainframe Connection Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MfConfirmationPort', '4001', 'Mainframe Connection Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MFOrderHistoryPort', '4002', 'Mainframe Connection Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MfPort', '3011', 'Mainframe Connection Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MfTrans', 'OT90', 'Mainframe Connection Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MfTransHistory', 'OT96', 'Mainframe Connection Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MultiDocsProxyUrl', 'http://testmultidocs.bekco.com/', 'MultiDocs Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('MultiDocsUrl', 'http://testmultidocs.bekco.com/', 'MultiDocs Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('OrderUpdateWatchPath', 'c:\test\entree\orderupdates\', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('PowerMenuAdminPassword', '9M9NC0', 'PowerMenu Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('PowerMenuAdminUsername', 'pniadmin', 'PowerMenu Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('PowerMenuGroupSetupUrl', 'http://bekpmwsq1.bekco.com:8443/main/Logon.aspx?Adminusername=pniadmin&amp;Adminpassword=9M9NC0N&amp;path=CUSTOMER', 'PowerMenu Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('PowerMenuLoginUrl', 'http://bekpmwsq1.bekco.com:8443/main/Logon.aspx?username={0}&amp;password={1}&amp;path=MAIN&amp;customerlist={2}&amp;order=true&amp;framed=false&amp;lang=ENG&amp;country=USA', 'PowerMenu Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('PowerMenuPermissionsUrl', 'http://bekpmwsq1.bekco.com:8443/main/Logon.aspx?Adminusername=pniadmin&amp;Adminpassword=9M9NC0N&amp;Username={0}&amp;path=USER', 'PowerMenu Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('PowerMenuWebServiceUrl', 'http://bekpmwsq1.bekco.com:8443/pmserviceendpoint.asmx', 'PowerMenu Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('PresentationUrl', 'http://corpmisdev2a.bekco.com:8080/', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('PurchaseOrdersGetLatestHowManyDays', '10', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQAccessConsumerUserName', 'accessclnt', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQAccessConsumerUserPassword', 'acspassclnt', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQAccessExchange', 'access_exchange', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQAccessPublisherUserName', 'accesspub', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQAccessPublisherUserPassword', 'acspasspub', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQAccessServer', 'qarmq.benekeith.com', 'RabbitMQ Settings: Servers', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQAccessVHost', 'access_update', 'RabbitMQ Settings: VHost', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQConfirmationErrorExchange', 'bek_commerce_confirmations_errors', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQConfirmationExchange', 'bek_commerce_confirmations', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQConfirmationServer', 'qarmq.benekeith.com', 'RabbitMQ Settings: Servers', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQConfirmationVHost', 'orders_statusnotification', 'RabbitMQ Settings: VHost', 0)

/*
	 Queues
*/

-- Access 
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQAccessQueue', 'access_update_queue', 'RabbitMQ Settings: Queue', 0)

-- Confirmations
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQConfirmationQueue', 'confirmations_debug_bakillins', 'RabbitMQ Settings: Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQConfirmationErrorQueue', 'confirmations_error', 'RabbitMQ Settings: Queue', 0)

-- Notifications
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQETAQueue', 'notifications_v2_bakillins_ETA', 'RabbitMQ Settings: Notifications Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQHasNewsQueue', 'notifications_v2_bakillins_hasnews', 'RabbitMQ Settings: Notifications Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationErrorQueue', 'notifications_errors', 'RabbitMQ Notifications Error Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationQueue', 'notifications_v1', 'RabbitMQ Settings: Notifications Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderConfirmationQueue', 'notifications_v2_bakillins_orderconfirmations', 'RabbitMQ Settings: Notification Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQPaymentConfirmationQueue', 'notifications_v2_bakillins_paymentconfirmations', 'RabbitMQ Settings: Queue', 0)

-- Push Notifications
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQPushMessagesQueue', 'notifications_push_messages_bakillins', 'RabbitMQ Settings: Queue', 0)

-- Requests
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderErrorQueue', 'orders_error', 'RabbitMQ Settings: Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQUpdateRequestQueue', 'orders_history', 'RabbitMQ Settings: Queue', 0)

-- Order Processing
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderQueue', 'orders_created_bakillins', 'RabbitMQ Settings: Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderReprocessQueue', 'orders_reprocess', 'RabbitMQ Settings: Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderHistoryQueue', 'orders_history', 'RabbitMQ Settings: Queue', 0)

-- Order Updates
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderUpdateQueue', 'orders_updates_dev_bakillins', 'RabbitMQ Settings: Queue', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQSpecialOrderUpdateQueue', 'specialorders_updates_dev_bakillins', 'RabbitMQ Settings: Queue', 0)

INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationConsumerUserName', 'notifyclnt', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationConsumerUserPassword', 'notifypassclnt', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationErrorExchange', 'notifications_errors', 'RabbitMQ Notifications Error Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationExchange', 'bek_commerce_notifications', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationPublisherUserName', 'notifypub', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationPublisherUserPassword', 'notifypasspub', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationServer', 'qarmq.benekeith.com', 'RabbitMQ Settings: Servers', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQNotificationVHost', 'notifications_notify', 'RabbitMQ Settings: VHost', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderConsumerUserName', 'orderclnt', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderConsumerUserPassword', 'ordpassclnt', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderCreatedExchange', 'bek_commerce_orders_created', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderErrorExchange', 'bek_commerce_orders_error', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderHistoryExchange', 'bek_commerce_orders_history', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderPublisherUserName', 'orderpub', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderPublisherUserPassword', 'ordpasspub', 'RabbitMQ Settings: Credentials', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderReprocessExchange', 'bek_commerce_orders_reprocess', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderServer', 'qarmq.benekeith.com', 'RabbitMQ Settings: Servers', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderUpdateExchange', 'bek_commerce_orderupdates', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQOrderVHost', 'orders_createupdate', 'RabbitMQ Settings: VHost', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQPushMessagesExchange', 'bek_commerce_push_messages', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQSpecialOrderUpdateExchange', 'bek_commerce_specialorderupdates', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RabbitMQUpdateRequestExchange', 'bek_commerce_historyrequest', 'RabbitMQ Settings: Exchange', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RecentItemsToKeep', '10', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RequireHttps', 'false', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('RunInternalServiceQueues', 'true', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ServiceEmailAddress', 'noreply@credera.com', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('SMTPHostName', 'localhost', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('SMTPPassword', '0', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('SMTPSendPort', '0', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('SmtpServer', 'localhost', 'Email Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('SMTPUsername', '0', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('TokenDuration', '8', 'Site Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('UNFIWhitelistBEKUsers', 'tcfox,jwames,pabrandt,jmmills,mdjoiner,jdhughes,dmderusha,bakillins,', 'UNFI Whitelisting', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('UNFIWhitelistCustomers', '742488', 'UNFI Whitelisting', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('UNFIWhitelistDSRs', '', 'UNFI Whitelisting', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('WebNowUrl', 'http://invoice.benekeith.com/webnow/index.jsp?action=filter&amp;username=anonymous&amp;drawer={branch}AR501&amp;tab={customer}&amp;field4={invoice}', 'Misc Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('WhiteListedBekUsers', 'tcfox,jwames,pabrandt,mdjoiner,jdhughes,corp-ssa-entreadmin,dmderusha,bakillins,jmmills,meiacomini', 'WhiteList: must be lowercase', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('WhiteListedBekUsersEnforced', 'false', 'WhiteList', 0)

-- Monitor Service Entries
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CheckLostOrders', 'true', 'Monitor Service Functions', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CheckLostOrdersStatus', 'Submitted', 'Monitor Service Functions', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CheckQueueHealth', 'true', 'Monitor Service Functions', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('QueuesToCheck', '
{targets:[{"server":"qarmq.benekeith.com",
          "username":"orderclnt",
		  "password":"ordpassclnt",
		  "virtualhost":"orders_statusnotification",
		  "logicalname":"order_update",
		  "queue":"matt_orders_updates",
		  "minimumconsumerswarningthreshold":2,
		  "maximummessageswarningthreshold":1}
]}', 'Monitor Service Functions', 0)

-- Contract List Changes
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ContractListDeleteBlockPrices', 'true', 'Contract List Changes', 0)

MERGE INTO [Configuration].[AppSettings] A
USING @AppSettings B ON (A.[Key] = B.[Key])
WHEN MATCHED THEN
    UPDATE SET A.[Comment] = B.[Comment], A.[Value] = B.[Value], A.[Disabled] = B.[Disabled]
WHEN NOT MATCHED THEN
    INSERT (Comment, [Key], Value, [Disabled]) 
	  VALUES(B.[Comment],B.[Key],B.[Value],B.[Disabled]);