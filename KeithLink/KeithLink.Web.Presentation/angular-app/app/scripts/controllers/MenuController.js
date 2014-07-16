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
    
    $scope.isAdmin = false;
    $scope.isBuyer = false;
    $scope.isPayer = false;


    $scope.login = function() {
      UserProfileService.getProfile().then(function() {
        $scope.currentUser = UserProfileService.profile;
      });
    };

    $scope.logout = function() {
      UserProfileService.profile = undefined;
      debugger;
    };


  }]);
