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
    'ui.router'
  ])
.config(function($stateProvider, $urlRouterProvider) {
  $stateProvider
    .state('menu', {
        templateUrl: 'views/menu.html'
    })
    .state('menu.home', {
        url: '/home',
        templateUrl: 'views/home.html',
        controller: 'HomeController'
    })
    .state('menu.catalog', {
        url: '/catalog',
        templateUrl: 'views/catalog.html',
        controller: 'CatalogController'
    // })
    // .state('list.item', {
    //     url: '/:item',
    //     templateUrl: 'templates/list.item.html',
    //     controller: function($scope, $stateParams) {
    //         $scope.item = $stateParams.item;
    //     }
    });
  $urlRouterProvider.when('', '/home');
  $urlRouterProvider.when('/', '/home');
});
  // .config(function ($routeProvider) {
  //   $routeProvider
  //     .when('/', {
  //       templateUrl: 'views/home.html',
  //       controller: 'HomeController'
  //     })
  //     .when('/catalog', {
  //       templateUrl: 'views/catalog.html',
  //       controller: 'CatalogController'
  //     })
  //     .otherwise({
  //       redirectTo: '/'
  //     });
  // });