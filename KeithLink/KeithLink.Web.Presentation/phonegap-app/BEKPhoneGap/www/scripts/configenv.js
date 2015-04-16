'use strict';

 angular.module('configenv', [])

.constant('ENV', {name:'prod',apiKey:'web_prod_v1',apiEndpoint:'https://shop.benekeith.com/api',loggingEnabled:false,googleAnalytics:'UA-58495462-1',mobileApp:true})

;