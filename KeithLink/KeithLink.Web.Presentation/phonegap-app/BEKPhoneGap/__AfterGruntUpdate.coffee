app.js

add PhonegapServices to .run

------------------------------------------------------------

index.html

Change "styles/main.css" to "css/main.css" at top

Remove "ng-app=bekapp" from <body>

Add these below lib scripts:
    <script type="text/javascript" charset="utf-8" src="cordova.js"></script>
    <script type="text/javascript" src="scripts/index.js"></script>

    <script>
    angular.element(document).ready(function() {
        app.initialize();
      angular.bootstrap(document, ['bekApp']);
    });
    </script>

add these Below resources:
    <script type="text/javascript" src="scripts/phonegapServices/PhonegapServices.js"></script>
    <script type="text/javascript" src="scripts/phonegapServices/PhonegapAuthenticationService.js"></script>
    <script type="text/javascript" src="scripts/phonegapServices/PhonegapListService.js"></script>
    <script type="text/javascript" src="scripts/phonegapServices/PhonegapCartService.js"></script>

------------------------------------------------------------

configenv.js

change apiEndpoint to: apiEndpoint:'https://shopqa.benekeith.com/api'

------------------------------------------------------------

then run in terminal:
compass compile www/scss/main.scss

------------------------------------------------------------

