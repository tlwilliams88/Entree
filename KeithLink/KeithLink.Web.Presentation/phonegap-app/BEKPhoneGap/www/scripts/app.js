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
    'LocalStorageModule', // HTML5 local storage
    'ui.router',
    'ui.bootstrap',
    'ui.sortable', // jquery ui list sorting (used on lists page)
    'shoppinpal.mobile-menu', // mobile sidebar menu
    'ngDragDrop', // jquery ui drag and drop (used on lists page)
    'infinite-scroll',
    'unsavedChanges', // throws warning to user when navigating away from an unsaved form
    'toaster', // user notification messages
    'angular-loading-bar', // loading indicator in the upper left corner
    'angularFileUpload', // csv file uploads for lists and orders
    'naif.base64', // base64 file uploads for images
    'fcsa-number',
    'ui.select2',
    'configenv'
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
.run(['$rootScope', '$state', '$log', 'toaster', 'AccessService', 'AuthenticationService', 'NotificationService', 'PhonegapServices', 'PhonegapPushService',
  function($rootScope, $state, $log, toaster, AccessService, AuthenticationService, NotificationService, PhonegapServices, PhonegapPushService) {

  $rootScope.displayMessage = function(type, message) {
    toaster.pop(type, null, message);
  };

  $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
    $log.debug('route: ' + toState.name);

    // check if route is protected
    if (toState.data && toState.data.authorize) {
      // check if user's token is expired
      if (!AccessService.isLoggedIn()) {
        AuthenticationService.logout();
        $state.go('register');
        event.preventDefault();
      }

      // check if user has access to the route
      if (!AccessService[toState.data.authorize]()) {
        $state.go('register');
        event.preventDefault();
      }
    }

    // redirect register page to homepage if logged in
    if (toState.name === 'register' && AccessService.isLoggedIn()) {
      PhonegapPushService.register();
      if ( AccessService.isOrderEntryCustomer() ) {
        $state.go('menu.home');
      } else {
        $state.go('menu.catalog.home');
      }

      event.preventDefault();
    }

  });

  $rootScope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState, fromParams) {
    if (AccessService.isOrderEntryCustomer()) {
      NotificationService.getUnreadMessageCount();
    }
  });

}]);

