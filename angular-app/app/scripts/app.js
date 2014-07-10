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
    .state('main', {
        url: '',
        templateUrl: 'views/main.html'
    })
    .state('main.home', {
        url: '/home',
        templateUrl: 'views/home.html',
        controller: 'HomeController'
    })
    .state('main.catalog', {
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