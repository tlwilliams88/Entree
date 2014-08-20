'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */

var GuestModalController = function ($scope, $modalInstance) {

  $scope.ok = function () {
    $modalInstance.close();
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
};


angular.module('bekApp')
  .controller('MenuController', ['$scope', '$modal', 'localStorageService', 'Constants', 'AuthenticationService', 'UserProfileService', 'AuthorizationService', 
    function ($scope, $modal, localStorageService, Constants, AuthenticationService, UserProfileService, AuthorizationService) {

    $scope.userProfile = UserProfileService.profile();
    $scope.currentLocation = UserProfileService.getCurrentLocation();

    $scope.loginInfo = {
      username: 'sabroussard@somecompany.com',
      password: 'L1ttleStev1e'
    };

    $scope.isLoggedIn = function() {
      return AuthorizationService.isLoggedIn();
    };

    $scope.isCustomer = function() {
      return AuthorizationService.isCustomer();
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

    $scope.print = function () {
      window.print(); 
    };
    
  }]);
