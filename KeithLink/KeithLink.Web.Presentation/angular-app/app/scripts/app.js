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
      templateUrl: 'views/searchresults.html'
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
      controller: function($scope, $stateParams) {
        $scope.categoryId = $stateParams.categoryId;
      }
    })
    // /catalog/brand/:brandId
    .state('menu.catalog.brand', {
      url: '/brand/:brandId',
      templateUrl: 'views/searchresults.html',
      controller: function($scope, $stateParams) {
        $scope.brandId = $stateParams.brandId;
      }
    });
  // redirect to /home route when going to '' or '/' paths
  $urlRouterProvider.when('', '/home');
  $urlRouterProvider.when('/', '/home');
});
