'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:AboutCtrl
 * @description
 * # AboutCtrl
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('AboutCtrl', function ($scope) {
    $scope.awesomeThings = [
      'HTML5 Boilerplate',
      'AngularJS',
      'Karma'
    ];
  });
