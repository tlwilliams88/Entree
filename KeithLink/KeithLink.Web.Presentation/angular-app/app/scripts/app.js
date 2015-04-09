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
    'unsavedChanges',         // throws warning to user when navigating away from an unsaved form
    'toaster',                // user notification messages
    'angular-loading-bar',    // loading indicator in the upper left corner
    'angularFileUpload',      // csv file uploads for lists and orders
    'fcsa-number',            // used for number validation
    'ui.select2',             // used for context menu dropdown in upper left corner
    'configenv'               // used to inject environment variables into angular through Grunt
  ])
.config(['$compileProvider', '$tooltipProvider', '$httpProvider', '$logProvider', 'localStorageServiceProvider', 'cfpLoadingBarProvider', 'ENV', 
  function($compileProvider, $tooltipProvider, $httpProvider, $logProvider, localStorageServiceProvider, cfpLoadingBarProvider, ENV) {
 
  // configure loading bar
  cfpLoadingBarProvider.includeBar = false;
 
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
 
}])
.run(['$rootScope', '$state', '$log', 'toaster', 'ENV', 'AccessService', 'AuthenticationService', 'NotificationService', 'ListService', 'CartService', '$window', '$location', 'PhonegapServices', 'PhonegapPushService',
  function($rootScope, $state, $log, toaster, ENV, AccessService, AuthenticationService, NotificationService, ListService, CartService, $window, $location, PhonegapServices, PhonegapPushService) {
 
  // helper method to display toaster popup message
  // takes 'success', 'error' types and message as a string
  $rootScope.displayMessage = function(type, message) {
    toaster.pop(type, null, message);
  };
 
  $rootScope.redirectUserToCorrectHomepage = function() {
    if ( AccessService.isOrderEntryCustomer() ) {
      $state.go('authorize.menu.home');
    } else {
      $state.go('authorize.menu.catalog.home');
    }
  };
 
  $rootScope.openExternalLink = function(url) {
    window.open(url, '_system');
  };
 
  /**********
  $stateChangeStart
  **********/
 
  $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
    $log.debug('route: ' + toState.name);
 
    // if token is empty and the state is restricted to logged in users, go to register
    if (!AccessService.isValidToken() && toState.data && toState.data.authorize) {
      $log.debug('Invalid token');
      $state.go('authorize.register');
      event.preventDefault();
      return;
    }

    if (AccessService.isPasswordExpired()) {
      $log.debug('User password expired');
      $state.go('authorize.changepassword');
      event.preventDefault();
    }

    // check if route is restricted to logged in users
    if (AccessService.isLoggedIn() && toState.data && toState.data.authorize) {
      
      // check if user has access to the route based on role and permissions
      if (!AccessService[toState.data.authorize]()) {
        $log.debug('User does not have access to the route');
        // redirect to correct homepage 
        if (toState.name === 'authorize.menu.home') {
          $state.go('authorize.menu.catalog.home');
        } else {
          $state.go('authorize.register');
        }
        event.preventDefault();
      }
    }
 
    // redirect register page to homepage if logged in
    if (toState.name === 'authorize.register' && AccessService.isValidToken()) {
      $log.debug('user logged in, redirecting to homepage');
      if (ENV.mobileApp) {  // ask to allow push notifications
        PhonegapPushService.register();
      }
      $rootScope.redirectUserToCorrectHomepage();
      event.preventDefault();
    }
 
  });
 
  /**********
  $stateChangeSuccess
  **********/
 
  $rootScope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState, fromParams) {

    // updates unread message count in header bar
    if (AccessService.isOrderEntryCustomer()) {
      NotificationService.getUnreadMessageCount();
    }
 
    // updates google analytics when state changes
    if (!$window.ga) {
      return;
    }
    $window.ga('send', 'pageview', { page: $location.path() });
 
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
      $state.go('authorize.register');
      event.preventDefault();
    }
  });
 
}]);