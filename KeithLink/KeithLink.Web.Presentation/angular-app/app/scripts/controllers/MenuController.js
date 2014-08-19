'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('MenuController', ['$scope', 'localStorageService', 'Constants', 'AuthenticationService', 'UserProfileService', function ($scope, localStorageService, Constants, AuthenticationService, UserProfileService) {

    $scope.userProfile = UserProfileService.profile();
    $scope.currentLocation = UserProfileService.getCurrentLocation();

    $scope.loginInfo = {
      username: 'sabroussard@somecompany.com',
      password: 'L1ttleStev1e'
    };

    $scope.isLoggedIn = function() {
      return AuthenticationService.verified();
    };

    $scope.login = function(loginInfo) {

      AuthenticationService.authenticateUser(loginInfo.username, loginInfo.password).then(function(token) {
        UserProfileService.getProfile(loginInfo.username).then(function(profile) {

          localStorageService.bind($scope, Constants.localStorage.userProfile, profile);

          var currentLocation = profile.stores[0];
          localStorageService.bind($scope, Constants.localStorage.currentLocation, currentLocation);

          $scope.showLoginForm = false;
        });
      });

    };

    $scope.logout = function() {
      AuthenticationService.logout();
      $scope.displayUserMenu = false;
    };

    $scope.changeLocation = function(newLocation) {
      UserProfileService.setCurrentLocation(newLocation);
    };

    $scope.print = function () {
      window.print(); 
    };
    
  }]);
