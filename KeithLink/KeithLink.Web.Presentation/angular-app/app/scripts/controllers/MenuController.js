'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('MenuController', ['$scope', 'AuthenticationService', 'UserProfileService', function ($scope, AuthenticationService, UserProfileService) {

    $scope.currentUser = UserProfileService.profile;

    $scope.loginInfo = {
      username: 'sabroussard@somecompany.com',
      password: 'L1ttleStev1e'
    };



    $scope.isValidUser = function() {
      return AuthenticationService.isLoggedIn();
    };

    $scope.login = function(loginInfo) {
      AuthenticationService.login(loginInfo.username, loginInfo.password);
    };

    $scope.login($scope.loginInfo);

    $scope.logout = function() {
      AuthenticationService.logout();
      $scope.displayUserMenu = false;
    };

    $scope.print = function () {
      window.print(); 
    };
    
  }]);
