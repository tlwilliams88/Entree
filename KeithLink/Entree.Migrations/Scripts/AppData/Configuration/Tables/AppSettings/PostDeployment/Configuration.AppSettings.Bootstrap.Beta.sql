DECLARE @AppSettings as TABLE 
( 
    [Key]      VARCHAR (50)  NOT NULL,
    [Value]    VARCHAR (MAX) NOT NULL,
    [Comment]  VARCHAR (MAX) NOT NULL,
    [Disabled] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC)
)

INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'AppName', 'Entree', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES('Site Settings', 'CartOrOrder2ListIdPurgeDays', '-7', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'PresentationUrl', 'http://shopbeta.benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'AllowedApiKeys', 'android_prod_v1,iphone_prod_v1,web_prod_v1', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'BrandAssetsUrl', 'https://shopbeta.benekeith.com/assets/brands', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'RequireHttps', 'true', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'RecentItemsToKeep', '10', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'ListItemDaysNew', '2', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'RunInternalServiceQueues', 'true', 0) 
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'TokenDuration', '7', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'IsProduction', 'true', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'OrderUpdateWatchPath', 's:\ecom\order_history\beta', 0) 
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings', 'PurchaseOrdersGetLatestHowManyDays', '10', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Site Settings: values are none, internal_only, all', 'EnableEtaForUsers', 'internal_only', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Item History - Used to geenrate item averages over a period of weeks', 'ItemHistoryAverageWeeks', '8', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtDomain', 'benekeith', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtPass', 'Qu3ryU$3r$', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtRoot', 'ou=KEC_Production,dc=benekeith,dc=com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtServer', 'bekkecad1.benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtUser', 'KEC-SVC-LDAP-Prod', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtUserContainer', 'Prod_Users', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtAccessGroupKbitAdmin', 'Kbit Admin', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtAccessGroupKbitCustomer', 'Kbit Customer', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtAccessGroupPowerMenuCustomer', 'PowerMenu Customer', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtRoleNameAccouting', 'Prod Accounting', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtRoleNameApprover', 'Prod Approver', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtRoleNameBuyer', 'Prod Buyer', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtRoleNameGuest', 'Prod Guest', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADExtRoleNameOwner', 'Prod Owner', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADIntDomain', 'bekco', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADIntPass', '@ud1tm3', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADIntRoleNameCorporateAdmin', 'CORP-LS-SYS-AC-Entree_Admins', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADIntRoot', 'dc=bekco,dc=com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADIntServer', 'ldap-bek.benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADIntUser', 'corp-ssa-secaudit', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADBadPwdCount', '3', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADLockoutDuration', '3', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Active Directory Settings', 'ADGuestOU', 'Prod_Users', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'WhiteList: must be lowercase', 'WhiteListedBekUsers', 'tcfox,jwames,pabrandt,mdjoiner,jdhughes,corp-ssa-entreadmin,dmderusha,bakillins,jmmills', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'WhiteList', 'WhiteListedBekUsersEnforced', 'true', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Elastic Search Settings', 'ElasticSearchURL', 'http://localhost:9200', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Elastic Search Settings', 'DefaultCategoryReturnSize', '2000', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Elastic Search Settings', 'DefaultProductReturnSize', '500', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Elastic Search Settings', 'MaxSortByPriceItemCount', '200', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Elastic Search Settings', 'ElasticSearchBatchSize', '500', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ElasticSearchAggregations', '	parentcategories:parentcategoryname_not_analyzed,categories:categoryname_not_analyzed,brands:brand_description_not_analyzed,allergens:nutritional.allergen.contains.keyword,dietary:nutritional.diet.diettype.keyword,itemspecs:itemspecification.keyword,nonstock:nonstock.keyword,mfrname:mfrname_not_analyzed,temp_zone:temp_zone.keyword', 'Elastic Search Settings', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('ElasticSearchTermSearchFields', '	keywords^1000,keywords.english^800,keywords.whitespace^700,name,name.english,description.english,categoryname,brand_description_not_analyzed,brand_description,vendor1,mfrname,mfrnumber,name_ngram_analyzed', 'Elastic Search Settings', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Elastic Search Settings', 'ElasticSearchDigitSearchFields', 'upc.ngram,gtin.ngram,itemnumber.ngram^5,mfrnumber.ngram^3', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Elastic Search Settings', 'ElasticSearchItemExcludeValues', 'D,!,W', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Elastic Search Settings', 'CategoryPrefixesToExclude', 'AA,ZZ,TW', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Commerce Server Settings', 'CS_SiteName', 'BEK_Commerce', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'CORS', 'CorsEnabledDomains', '*', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'CORS', 'CorsEnabledHeaders', '*', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'CORS', 'CorsEnabledMethods', '*', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Amazon Settings', 'AmazonSnsAccessKey', 'AKIAJ74TCMZZSQRJNDNQ', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Amazon Settings', 'AmazonSnsSecretKey', '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', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Amazon Settings', 'AmazonSnsMobilePlatformAppArnIOS', 'arn:aws:sns:us-east-1:951996173818:app/APNS/BEK_APNS', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Amazon Settings', 'AmazonSnsMobilePlatformAppArnAndroid', 'arn:aws:sns:us-east-1:951996173818:app/GCM/BEK_GCM', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Amazon Settings', 'AmazonSnsMessageToDisableOn', 'Endpoint is disabled', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Servers', 'RabbitMQAccessServer', 'localhost', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Servers', 'RabbitMQConfirmationServer', 'localhost', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Servers', 'RabbitMQNotificationServer', 'localhost', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Servers', 'RabbitMQOrderServer', 'localhost', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: VHost', 'RabbitMQAccessVHost', 'access_update', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: VHost', 'RabbitMQOrderVHost', 'orders_createupdate', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: VHost', 'RabbitMQConfirmationVHost', 'orders_statusnotification', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: VHost', 'RabbitMQNotificationVHost', 'notifications_notify', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQAccessExchange', 'access_exchange', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQOrderCreatedExchange', 'bek_commerce_orders_created', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQOrderErrorExchange', 'bek_commerce_orders_error', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQOrderReprocessExchange', 'bek_commerce_orders_reprocess', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQOrderHistoryExchange', 'bek_commerce_orders_history', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQUpdateRequestExchange', 'bek_commerce_historyrequest', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQOrderUpdateExchange', 'bek_commerce_orderupdates', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQSpecialOrderUpdateExchange', 'bek_commerce_specialorderupdates', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQConfirmationExchange', 'bek_commerce_confirmations', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQConfirmationErrorExchange', 'bek_commerce_confirmations_errors', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQNotificationExchange', 'bek_commerce_notifications', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQNotificationExchangeV2', 'bek_commerce_notifications_v2', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Exchange', 'RabbitMQPushMessagesExchange', 'bek_commerce_push_messages', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQNotificationPublisherUserName', 'notifypub', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQNotificationPublisherUserPassword', 'notifypasspub', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQAccessConsumerUserName', 'accessclnt', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQAccessConsumerUserPassword', 'acspassclnt', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQAccessPublisherUserName', 'accesspub', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQAccessPublisherUserPassword', 'acspasspub', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQOrderConsumerUserName', 'orderclnt', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQOrderConsumerUserPassword', 'ordpassclnt', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQOrderPublisherUserName', 'orderpub', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQOrderPublisherUserPassword', 'ordpasspub', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQNotificationConsumerUserName', 'notifyclnt', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Credentials', 'RabbitMQNotificationConsumerUserPassword', 'notifypassclnt', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQAccessQueue', 'access_update_queue', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQOrderQueue', 'orders_created', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQSpecialOrderUpdateQueue', 'specialorders_updates', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQOrderErrorQueue', 'orders_error', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQOrderHistoryQueue', 'orders_history', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQUpdateRequestQueue', 'orders_historyrequest', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQOrderReprocessQueue', 'orders_reprocess', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQOrderUpdateQueue', 'orders_updates', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQNotificationQueue', 'notifications_v1', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQOrderConfirmationQueue', 'notifications_v2_orderconfirmations', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQETAQueue', 'notifications_v2_eta', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQHasNewsQueue', 'notifications_v2_hasnews', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQPaymentConfirmationQueue', 'notifications_v2_paymentconfirmation', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQPushMessagesQueue', 'notifications_push_messages', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQConfirmationQueue', 'confirmations', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'RabbitMQ Settings: Queue', 'RabbitMQConfirmationErrorQueue', 'confirmations_error', 0)	
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'MultiDocs Settings', 'MultiDocsUrl', 'http://multidocs.benekeith.com/', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'MultiDocs Settings', 'MultiDocsProxyUrl', 'https://shopbeta.benekeith.com/multidocsurl/', 0)    
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'SmtpServer', 'kecampas1.benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'SMTPHostName', 'kecampas1.benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'ServiceEmailAddress', 'BekEntreeBeta@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'FailureEmailAddress', 'jwames@benekeith.com,mdjoiner@benekeith.com,bakillins@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'FromEmailAddress', 'BekEntreeBeta@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'FAQ_ContactEmail', 'faqinfo@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'FAM_ContactEmail', 'amainfo@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'FDF_ContactEmail', 'dfwinfo@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'FHS_ContactEmail', 'fhsinfo@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'FLR_ContactEmail', 'flrinfo@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'FOK_ContactEmail', 'okinfo@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'FSA_ContactEmail', 'fsainfo@benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Email Settings', 'ClientSettingsProvider.ServiceUri', '', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Mainframe Connection Settings', 'MfAddress', '192.168.20.12', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Mainframe Connection Settings', 'MfConfirmationPort', '4101', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Mainframe Connection Settings', 'MFOrderHistoryPort', '4102', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Mainframe Connection Settings', 'MfPort', '3010', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Mainframe Connection Settings', 'MfTrans', 'OT30', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Mainframe Connection Settings', 'MfTransHistory', 'OT35', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Mainframe Connection Settings', 'EntreeCollectorType', '5', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Cache setttings', 'AddServerNameToHeaderResponse', 'false', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'imaging settings', 'ImagingServerUrl', 'http://bekimage.bekco.com:8080/integrationserver_937/', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'imaging settings', 'ImagingUserName', 'corp-ssa-imgentree', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'imaging settings', 'ImagingUserPassword', 'imagenow67', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'imaging settings', 'ImagingViewId', '200000054R_001QL95EY9CM', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'BillPay Settings: this setting must be in a 24 hour format', 'BillPayCutOffTime', '14:00:00', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'PowerMenu Settings', 'PowerMenuAdminUsername', 'pniadmin', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'PowerMenu Settings', 'PowerMenuAdminPassword', '9M9NC0N', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'PowerMenu Settings', 'PowerMenuWebServiceUrl', 'https://emenumanage.benekeith.com:8443/pmserviceendpoint.asmx', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'PowerMenu Settings', 'PowerMenuPermissionsUrl', 'https://emenumanage.benekeith.com:8443/main/Logon.aspx?Adminusername=pniadmin&amp;Adminpassword=9M9NC0N&amp;Username={0}&amp;path=USER', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'PowerMenu Settings', 'PowerMenuGroupSetupUrl', 'https://emenumanage.benekeith.com:8443/main/Logon.aspx?Adminusername=pniadmin&amp;Adminpassword=9M9NC0N&amp;path=CUSTOMER', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'PowerMenu Settings', 'PowerMenuLoginUrl', 'https://emenumanage.benekeith.com:8443/main/Logon.aspx?username={0}&amp;password={1}&amp;path=MAIN&amp;customerlist={2}&amp;order=true&amp;framed=false&amp;lang=ENG&amp;country=USA', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Content Management Settings', 'MarketingBranchItemCount', '3', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Content Management Settings', 'MarketingContentUrl', 'http://www.benekeith.com/api/app', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Content Management Settings', 'MarketingTotalItemCount', '6', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Misc Settings', 'WebNowUrl', 'http://invoice.benekeith.com/webnow/index.jsp?action=filter&amp;username=anonymous&amp;drawer={branch}AR501&amp;tab={customer}&amp;field4={invoice}', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Misc Settings', 'LogSystemPerformanceWithErrors', 'true', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Misc Settings', 'CacheServerEndpoints', 'http://bekkecbas1:5006,http://bekkecbas2:5006', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Misc Settings', 'EntreeSiteURL', 'https://shopbeta.benekeith.com', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Order Service Functions', 'OrderServiceMakeKDOELogFiles', 'true', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Order Service Functions', 'OrderServiceKDOELogPath', 'c:\test\entree\kdoefiles', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Order Service Functions', 'OrderServiceKDOELogPre', 'rb', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Order Service Functions', 'OrderServiceKDOELogPost', '01', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Order Service Functions', 'OrderServiceKDOELogExtension', 'txt', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'UNFI Whitelisting', 'UNFIWhitelistBranches', '', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'UNFI Whitelisting', 'UNFIWhitelistDSRs', 'pllemieux,ragangler,meiacomini,sclabarba', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'UNFI Whitelisting', 'UNFIWhitelistCustomers', '', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'UNFI Whitelisting', 'UNFIWhitelistBEKUsers', 'tcfox,jwames,pabrandt,jmmills,mdjoiner,jdhughes,dmderusha,bakillins,bplynch,semcwilliams,tfyater,kabell,hknewman,dbentancourt,raboyd,lamccaskill,armoon,bjflickner,jlashley,kciacomini', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'DB Has Changed (change this to make clients reset)', 'DBChangeValue', ABS(Checksum(NewID()) % 1000), 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Export Settings', 'ExportAddCustomer', '', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Export Settings', 'ExportAddTitle', '', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Catalog Service Entries', 'CatalogServiceUnfiImagesIxOneAuthToken', 'JOdavyGM8uyKAbrbhAhQRI3f7egwY_MoL2SZN_qPKAI3AmJ0irpD4Sc08eMJq_k9VOX8DuslpDBUFE23pRuCtNjQZbLpmHuq23ToHAmck8Q3V0a_QMV2comjb-FmF0ddU1OMcdGvA-8gb3YRsldYGgGx1nkyIUY1cnUtQ369RobpzWjm61Yox3-4Yf-D-8Gj2ssH0-cmu8ANyxbOvHgxFfEfJ3ZTEsI2kf1xiTOP-pvqAFkAFDWbxJcflmF2mYsphkUgNJ3h7HDLqfjaNLMEHP8NHCt0Y7cT9ynVgntP3Gk7we1XSklacLq-xAdd5WF8l7X2CBqk9fQyuREw8_ZFpzvJv9jp5M-zxCPys8cO6WLstCHELoBovLiao7MKugAswPdXpqqe4KhVUvWffGRlGmUzceawqRIQRkrkhd14kEomvYLTTvMqyAHAoKeNT0Z53aQcOJp4A58DWDSH3KMnaGPkLbUsMx0y6-78Tz9ylBb5SChPMnnoqq_pXCTkXCMkhpvMDbTcE94zHH2Bd8Pqpw_M13FuNu-HAzMby_Sfw01q4IjN0ZgKzc3aW9TNNMtoyiqk3LhRag76QvG9qozp5CNAK6igmd8iHyMhAsy6MZyTYEFvaARnv6RSKd69JY4YBEDKdNo11E_6ITRLGoFiUYkNicXVNYYGzIdqBInVyJvGuoK_dh11rt4i_xqX_KNkRB0QhfQCdPYCHPZC4q65CdLdlRWb2t-Xf8KSQ1ZiDSxOcXbj31c32nWibbzlgcimzkyu2f0sEf3-mAlxiwkBgWnzxVet8QXiwC40Bx8_0Bz_49CArd19m9o9obhn91rm_pQaOb1ptgQGq6Ian3HbJxlONhzEmE1ZNn8YmIA6q8wir6rwrVKgu3sTBxBWUA8OdMTi7oBciWEP0C6_ry0Lu7DnOP09lykDATx9X6jJzx5IbQyokrPoigeuYWLJhiZX38qKlME42A0BThdbc5lw5A6orkJraiZVkWwvvZ6xCD_2wMtgGnR4at1GmFXENnJAqLO8x2DoTwwKmkuA6pEDct0J_EGsDpmbP2wEAYD9HtWzj304XO9YlSmEEXaDXKr99_B3BgQNQL2JKOoPqPwXyFfV_EAVvLGPho9r56TFNbKbOe7NHUcKxH2VJzfvDSgPqIfCrAwbhJsM-3bRI_dx_BpvrGcD7l3KwAZZrsTelU95msh5aSkzpy6KqzHkiSrMHRjEEHPmGABjy0MwNAhC8kI86yX4NIyiGyaNe9SC8FWjigRcRHN7QTtIDG25mRTH_dCBkazVeTA7t-O3RCFfKJOPsTe7nfhuoqT3LznxRPt2ERqBd7Xl4CkxlvhzjrZxYsCQQyL_3tmBANdQQcolf9mh4y8aq5pUeNjeGmNMFwFPQo9NwiQe04UbjCKckB21b0Thy5xwNMGdoyWA3KPnf_kXXQ_p-ZqvOcycW8n8CJJuWEmvllNiDnop9mfGCPeUXi0wALUeYdAbL8NxFc-DQLMH5JF8440wchrPmreWrq4SC68E3m9q5FJsgWtH6AC8FP94sXBlx2YwjS158vp2x67-GZZUD9Ov8qRkCxW4GcHPm7Rv2mvKAMMakPRyDBsir0zIMVdN728VHjx4iMalW_6lx3wgIfQPxW6Lg6gVz_Rc5WCILeQ1Ij5M-9_vUy5UhDMcJZvVw_UU1ofvaJDLuuzmJWNctir9E-D3xy8Y1-7NATvzmyMJdX9ASWlMtUrGjssLNCxrhk9OTbhPUS8AkGF3xchgmNAdEJ3u1-KV7gCrYNtzfjZHwoliklM0VtU_HvmWKbdMunXlGAn1t-H3nKUdAa6awc0Hs2FfwLYKuie2WmOp56MkEvCDgisdzxru0coZSGuhTCiBJ0f5rDDvy1grwuRt-BydQ_XiUXWdULEfxoJ8eiRZM_7Sfmg3roXk74AGLQ', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Catalog Service Entries', 'CatalogServiceUnfiImagesMakeThumbnails', 'false', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Catalog Service Entries', 'CatalogServiceUnfiImagesProcessTime', '00:00', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Catalog Service Entries (Image type specifier, comma-delimited, the first one they have is also what we call primary)', 'CatalogServiceUnfiImagesIxOneImagesWeTake', 'CNN1,CIN1,C1N1,C7N1', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Catalog Service Entries', 'CatalogServiceUnfiImagesRepo', '\\bekmain\CMIS-IFDA_Repository\ProductImages\Channels\IX-One', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Catalog Service Entries', 'CatalogServiceUnfiImagesScaleX', '250', 0)
INSERT INTO @AppSettings (Comment, [Key], Value, [Disabled]) VALUES( 'Catalog Service Entries', 'CatalogServiceUnfiImagesScaleY', '250', 0)

-- Monitor Service Entries
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CheckLostOrders', 'true', 'Monitor Service Functions', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CheckLostOrdersStatus', 'Submitted', 'Monitor Service Functions', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('CheckQueueHealth', 'true', 'Monitor Service Functions', 0)
INSERT @AppSettings ([Key], [Value], [Comment], [Disabled]) VALUES ('QueuesToCheck', '{targets:[
{"server":"betarmq.benekeith.com",
 "username":"notifyclnt",
 "password":"notifypassclnt",
 "virtualhost":"notifications_notify",
 "logicalname":"notifications_eta",
 "queue":"notifications_eta",
 "minimumconsumerswarningthreshold":2,
 "maximummessageswarningthreshold":100},
{"server":"betarmq.benekeith.com",
 "username":"notifyclnt",
 "password":"notifypassclnt",
 "virtualhost":"notifications_notify",
 "logicalname":"notifications_hasnews",
 "queue":"notifications_hasnews",
 "minimumconsumerswarningthreshold":2,
 "maximummessageswarningthreshold":100},
{"server":"betarmq.benekeith.com",
 "username":"notifyclnt",
 "password":"notifypassclnt",
 "virtualhost":"notifications_notify",
 "logicalname":"notifications_orderconfirmations",
 "queue":"notifications_orderconfirmations",
 "minimumconsumerswarningthreshold":2,
 "maximummessageswarningthreshold":100},
{"server":"betarmq.benekeith.com",
 "username":"notifyclnt",
 "password":"notifypassclnt",
 "virtualhost":"notifications_notify",
 "logicalname":"notifications_paymentconfirmation",
 "queue":"notifications_paymentconfirmation",
 "minimumconsumerswarningthreshold":2,
 "maximummessageswarningthreshold":100}
{"server":"betarmq.benekeith.com",
 "username":"notifyclnt",
 "password":"notifypassclnt",
 "virtualhost":"notifications_notify",
 "logicalname":"notifications_push_messages",
 "queue":"notifications_push_messages",
 "minimumconsumerswarningthreshold":2,
 "maximummessageswarningthreshold":100}
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
