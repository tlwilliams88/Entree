'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('MenuController', ['$scope', 'UserProfileService', function ($scope, UserProfileService) {
    
    $scope.isAdmin = UserProfileService.hasAdminRole();
    $scope.isBuyer = UserProfileService.hasBuyerRole();
    $scope.isPayer = UserProfileService.hasPayerRole();

  }]);
