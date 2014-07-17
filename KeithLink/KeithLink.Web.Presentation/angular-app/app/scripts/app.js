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
    // 'ngTouch',
    'ui.router',
    'ui.bootstrap'
  ])
.config(function($stateProvider, $urlRouterProvider) {
  // the $stateProvider determines path urls and their related controllers
  $stateProvider
    .state('menu', {
      abstract: true, // path that cannot be navigated to directly, it can only be accessed by child views
      templateUrl: 'views/menu.html',
      controller: 'MenuController'
    })
    // /home
    .state('menu.home', {
      url: '/home',
      templateUrl: 'views/home.html',
      controller: 'HomeController'
    })
    .state('menu.catalog', {
      abstract: true,
      url: '/catalog',
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
      url: '/products',
      template: '<div ui-view=""></div>'
    })
    // /catalog/products
    .state('menu.catalog.products.home', {
      url: '',
      templateUrl: 'views/searchresults.html',
      controller: 'SearchController'
    })
    // /catalog/products/:itemId (item details page)
    .state('menu.catalog.products.details', {
      url: '/:itemId',
      templateUrl: 'views/itemdetails.html',
      controller: 'ItemDetailsController'
    })
    // /catalog/category/:categoryId
    .state('menu.catalog.category', {
      url: '/category/:categoryId',
      templateUrl: 'views/searchresults.html',
      controller: 'SearchController'
      //function($scope, $stateParams) {
        //$scope.categoryId = $stateParams.categoryId;
      //}
    })
    // /catalog/brand/:brandId
    .state('menu.catalog.brand', {
      url: '/brand/:brandId',
      templateUrl: 'views/searchresults.html',
      controller: function($scope, $stateParams) {
        $scope.brandId = $stateParams.brandId;
      }
    });
  $stateProvider
    .state('404', { 
      url: '/404',
      templateUrl: 'views/404.html'
    });
  // redirect to /home route when going to '' or '/' paths
  $urlRouterProvider.when('', '/home');
  $urlRouterProvider.when('/', '/home');
  $urlRouterProvider.otherwise('/404');
})
.run(['$rootScope', 'UserProfileService', 'ApiService', function($rootScope, UserProfileService, ApiService) {

  ApiService.getEndpointUrl().then(function(response) {
    ApiService.endpointUrl = 'http://' + response.data.ClientApiEndpoint;
  });

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
