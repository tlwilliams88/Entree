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


    $scope.currentUser = UserProfileService.getProfile();


    var isLoggedIn = false;
    $scope.isLoggedIn = function() {
      return isLoggedIn;
    };

    $scope.login = function() {
      isLoggedIn = true;
    };

    $scope.logout = function() {
      $scope.displayUserMenu = false;
      isLoggedIn = false;
    };


    $scope.print = function () {
      window.print(); 
    };
    
  }]);
