'use strict';
 
 
/**
 * @ngdoc overview
 * @name bekApp
 * @description
 * # bekApp
 *
 * Main module of the application.
 */
angular
  .module('bekApp', [
    'ngAnimate',
    // 'ngCookies',
    'ngResource',
    'ngSanitize',
    'ngTouch',
    'LocalStorageModule',     // HTML5 local storage
    'ui.router',
    'ui.bootstrap',
    'ui.sortable',            // jquery ui list sorting (used on lists page)
    'angular-carousel',
    'ngDragDrop',             // jquery ui drag and drop (used on lists page)
    'infinite-scroll',
    'fsm',
    'unsavedChanges',         // throws warning to user when navigating away from an unsaved form
    'toaster',                // user notification messages
    'angular-loading-bar',    // loading indicator in the upper left corner
    'angularFileUpload',      // csv file uploads for lists and orders
    'fcsa-number',            // used for number validation
    'ui.select2',
    'blockUI',            // used for context menu dropdown in upper left corner
    'sticky',
    'configenv',               // used to inject environment variables into angular through Grunt
    'angulartics', 
    'angulartics.google.analytics'
  ])
.config(['$compileProvider', '$tooltipProvider', '$httpProvider', '$logProvider', 'localStorageServiceProvider', 'cfpLoadingBarProvider', 'ENV', 'blockUIConfig', '$analyticsProvider',
  function($compileProvider, $tooltipProvider, $httpProvider, $logProvider, localStorageServiceProvider, cfpLoadingBarProvider, ENV, blockUIConfig, $analyticsProvider) {
 
  //googleAnalyticsCordovaProvider.trackingId = 'UA-62498504-2';
  //googleAnalyticsCordovaProvider.period = 20; // default: 10 (in seconds)
  //googleAnalyticsCordovaProvider.debug = true; // default: false

  // configure loading bar
  cfpLoadingBarProvider.includeSpinner = false;
  // cfpLoadingBarProvider.latencyThreshold = 500;
 
  // configure logging
  $logProvider.debugEnabled(ENV.loggingEnabled);
 
  // set local storage prefix
  localStorageServiceProvider.setPrefix('bek');
 
  // add authentication headers and Api Url
  $httpProvider.interceptors.push('AuthenticationInterceptor');
 
  // group multiple aysnc methods together to only run through one digest cycle
  $httpProvider.useApplyAsync(true);
 
  $compileProvider.debugInfoEnabled(false);
 
  // fix for ngAnimate and ui-bootstrap tooltips
  $tooltipProvider.options({animation: false});

  blockUIConfig.requestFilter = function(config) {
    if (config.data && config.data.message) {
      return config.data.message;
    } else if (config.data && typeof config.data === 'string' && config.data.indexOf('message')) { // authen
      return config.data.substr(config.data.indexOf('message')+8);
    } else if (config.url === '/invoice/payment' && config.method === 'POST') { // submit payments
      return 'Submitting payments...';
    } else if (config.method === 'PUT' && config.url.indexOf('/active') === -1) { // show overlay for all PUT requests except for active cart 
      return 'Saving...';
    } else if (config.method === 'DELETE') {
      return 'Deleting...';
    } else {
      return false;
    }
  };  
}])
.run(['$rootScope', '$state', '$log', 'toaster', 'ENV', 'AccessService', 'NotificationService', 'ListService', 'CartService', 'UserProfileService', '$window', '$location', 'PhonegapServices', 'PhonegapPushService',
  function($rootScope, $state, $log, toaster, ENV, AccessService, NotificationService, ListService, CartService, UserProfileService, $window, $location, PhonegapServices, PhonegapPushService) {
 
  // helper method to display toaster popup message
  // takes 'success', 'error' types and message as a string
  $rootScope.displayMessage = function(type, message) {
    toaster.pop(type, null, message);
  };
 
  $rootScope.redirectUserToCorrectHomepage = function() {
    if ( AccessService.isOrderEntryCustomer() ) {
      $state.go('menu.home');
    } else {
      $state.go('menu.catalog.home');
    }
  };
 
  $rootScope.openExternalLink = function(url) {
    window.open(url, '_system');
  };
 
  /**********
  $stateChangeStart
  **********/
 
  var bypass;
  $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {

    function isStateRestricted(stateData) {
      return stateData && stateData.authorize;
    }
    function processValidStateChange() {
      bypass = true;
      $state.go(toState, toParams);
    }
    function validateStateForLoggedInUser() {
      // redirect to homepage, if restricted state and user not authorized OR going to register page
      if (AccessService.isPasswordExpired() && toState.name !== 'changepassword') {
        $state.go('changepassword');
      } else if ( ( isStateRestricted(toState.data) && !AccessService[toState.data.authorize]() ) || toState.name === 'register' ) {
        $log.debug('redirecting to homepage');

        // ask to allow push notifications
        if (toState.name === 'register' && ENV.mobileApp) {
          PhonegapPushService.register();
        }

        $rootScope.redirectUserToCorrectHomepage();
      } else {
        processValidStateChange();
      }
    }

    if (bypass) { 
      $log.debug('route: ' + toState.name);
      bypass = false;
      return; 
    }

    event.preventDefault();

    // Validate the state the user is trying to access
    
    if (AccessService.isLoggedIn()) {
      $log.debug('user logged in');
      validateStateForLoggedInUser();

    } else if (AccessService.isValidToken()) {
      $log.debug('user has token, getting profile');
      UserProfileService.getCurrentUserProfile()
        .then(validateStateForLoggedInUser);

    } else { // no token, no profile
      if (isStateRestricted(toState.data)) {
        AccessService.clearLocalStorage();
        $log.debug('user NOT logged in, redirecting to register page');
        $state.go('register');
      } else {
        processValidStateChange();
      }
    }
 
  });
 
  /**********
  $stateChangeSuccess
  **********/
 
  $rootScope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState, fromParams) {

    $log.debug('state change success');

    // Pull Mandatory notifications for header bar
       var notificationParams = {     
    size: 50,
    from: 0,
        filter: {
        field: "mandatory",
        value: "true",
        filter:[
        {
        field: "messagereadutc",
        value: 'null'
        }
        ]
      }
    };

    // updates unread message count in header bar
    if (AccessService.isOrderEntryCustomer()) {
      NotificationService.getMessages(notificationParams);
      NotificationService.getUnreadMessageCount();
    }
 
    // remove lists and carts from memory
    if (fromState.data && toState.data) {
      if (fromState.data.saveLists && !toState.data.saveLists) {
        $log.debug('erasing lists and labels');
        ListService.eraseCachedLists();
      }
      if (fromState.data.saveCarts && !toState.data.saveCarts) {
        $log.debug('erasing carts');
        CartService.cartHeaders = [];
      }
    }
  });
 
  /**********
  $stateChangeError
  **********/
  
  $rootScope.$on('$stateChangeError', function(event, toState, toParams, fromState, fromParams, error){
    $log.debug(error);

    if (error.status === 401) {
      $state.go('register');
      event.preventDefault();
    }
  });
}]);