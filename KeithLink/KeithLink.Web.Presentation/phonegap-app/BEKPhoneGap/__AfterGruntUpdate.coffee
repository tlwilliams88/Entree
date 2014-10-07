app.js

, 'PhonegapServices'
, PhonegapServices

------------------------------------------------------------

index.html

Change "styles/main.css" to "css/main.css" at top

Remove "ng-app=bekapp" from <body>

Below lib scripts:
    <script type="text/javascript" charset="utf-8" src="cordova.js"></script>
    <script type="text/javascript" src="scripts/index.js"></script>

    <script>
    angular.element(document).ready(function() {
        app.initialize();
      angular.bootstrap(document, ['bekApp']);
    });
    </script>

Below resources:
    <script type="text/javascript" src="scripts/phonegapServices/PhonegapServices.js"></script>
    <script type="text/javascript" src="scripts/phonegapServices/PhonegapAuthenticationService.js"></script>
    <script type="text/javascript" src="scripts/phonegapServices/PhonegapListService.js"></script>
    <script type="text/javascript" src="scripts/phonegapServices/PhonegapCartService.js"></script>

------------------------------------------------------------

configenv.js

change apiEndpoint to: apiEndpoint:'http://devapi.bekco.com'

------------------------------------------------------------

then do:
compass compile www/scss/main.scss

------------------------------------------------------------

IF HTTPS.... DO THIS FOR NOW

then change this in build/platforms/ios/BEKPhoneGapApp/Classes/AppDelegate.m after doing a grunt phonegap:build:ios

@implementation NSURLRequest(DataController)
+ (BOOL)allowsAnyHTTPSCertificateForHost:(NSString *)host
{
    return YES; 
}
@end