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
    'ngCookies',
    'ngResource',
    'ngSanitize',
    'ngTouch',
    'ngRoute'
  ])
  .config(function ($routeProvider) {
    $routeProvider
      .when('/', {
        templateUrl: 'views/home.html',
        controller: 'HomeController'
      })
      .when('/catalog', {
        templateUrl: 'views/catalog.html',
        controller: 'CatalogController'
      })
      .otherwise({
        redirectTo: '/'
      });
  });