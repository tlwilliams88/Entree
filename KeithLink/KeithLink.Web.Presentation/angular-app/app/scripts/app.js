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
    // 'ngAnimate',
    // 'ngCookies',
    // 'ngResource',
    // 'ngSanitize',
    'ngTouch',
    'ui.router',
    'ui.bootstrap'
  ])
.config(function($stateProvider, $urlRouterProvider) {
  // the $stateProvider determines path urls and their related controllers
  $stateProvider
    .state('login', {
      url: '/login/',
      templateUrl: 'views/login.html'
      // controller: 'MenuController'
    })
    .state('menu', {
      abstract: true, // path that cannot be navigated to directly, it can only be accessed by child views
      templateUrl: 'views/menu.html',
      controller: 'MenuController'
    })
    // /home
    .state('menu.home', {
      url: '/home/',
      templateUrl: 'views/home.html',
      controller: 'HomeController'
    })
    .state('menu.catalog', {
      abstract: true,
      url: '/catalog/',
      template: '<div ui-view=""></div>'
    })
    // /catalog
    .state('menu.catalog.home', {
      url: '',
      templateUrl: 'views/catalog.html',
      controller: 'CatalogController'
    })
    .state('menu.catalog.products', {
      abstract: true,
      url: 'products/',
      template: '<div ui-view=""></div>'
    })
    // /catalog/:type/:id
    .state('menu.catalog.products.list', {
      url: ':type/:id/',
      templateUrl: 'views/searchresults.html',
      controller: 'SearchController'
    })
    // /catalog/products/:itemNumber (item details page)
    .state('menu.catalog.products.details', {
      url: ':itemNumber/',
      templateUrl: 'views/itemdetails.html',
      controller: 'ItemDetailsController'
    });

  $stateProvider
    .state('404', { 
      url: '/404/',
      templateUrl: 'views/404.html'
    });
  // redirect to /home route when going to '' or '/' paths
  $urlRouterProvider.when('', '/home');
  $urlRouterProvider.when('/', '/home');
  $urlRouterProvider.otherwise('/404');


  $urlRouterProvider.rule(function ($injector, $location) {
    var path = $location.url();

    // check to see if the path already has a slash where it should be
    if ('/' === path[path.length - 1] || path.indexOf('/?') > -1) {
        return;
    }
    if (path.indexOf('?') > -1) {
        return path.replace('?', '/?');
    }

    return path + '/';
  });
})
.run(['$rootScope', 'UserProfileService', 'ApiService', function($rootScope, UserProfileService, ApiService) {

  ApiService.endpointUrl = 'http://devapi.bekco.com';
  // ApiService.getEndpointUrl().then(function(response) {
  //   ApiService.endpointUrl = 'http://' + response.data.ClientApiEndpoint;
  // });

  $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
    // debugger;
    // if (!Auth.authorize(toState.data.access)) {
    //   $rootScope.error = 'Access denied';
    //   event.preventDefault();

    //   if(fromState.url === '^') {
    //     if(Auth.isLoggedIn())
    //       $state.go('user.home');
    //     else {
    //       $rootScope.error = null;
    //       $state.go('anon.login');
    //     }
    //   }
    // }
  });
}]);
