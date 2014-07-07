'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('MainCtrl', function ($scope) {
    $scope.awesomeThings = [
      'HTML5 Boilerplate',
      'AngularJS',
      'Karma'
    ];
  });
