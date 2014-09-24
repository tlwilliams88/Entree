'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MenuController', ['$scope', '$state', '$modal', 'branches', 'toaster', 'Constants', 'AuthenticationService', 'UserProfileService', 'AccessService', 
    function ($scope, $state, $modal, branches, toaster, Constants, AuthenticationService, UserProfileService, AccessService) {

    $scope.$state = $state;

    $scope.userProfile = UserProfileService.profile();
    $scope.userBar = {};
    $scope.userBar.universalSearchTerm = '';

    refreshAccessPermissions();
    
    $scope.changeLocation = function() {
      UserProfileService.setCurrentLocation($scope.currentLocation);
    };

    $scope.branches = branches;
    $scope.currentLocation = UserProfileService.profile().branchid;
    $scope.changeLocation();

    $scope.logout = function() {
      AuthenticationService.logout();
      refreshAccessPermissions();

      $state.transitionTo('register');
      $scope.displayUserMenu = false;
    };

    $scope.print = function () {
      window.print(); 
    };

    function refreshAccessPermissions() {
      $scope.isLoggedIn = AccessService.isLoggedIn();
      $scope.isOrderEntryCustomer = AccessService.isOrderEntryCustomer();
      
      $scope.canBrowseCatalog = AccessService.canBrowseCatalog();
      $scope.canManageLists = AccessService.canManageLists();
      $scope.canCreateOrders = AccessService.canCreateOrders();
      $scope.canSubmitOrders = AccessService.canSubmitOrders();
      $scope.canPayInvoices = AccessService.canPayInvoices();
      $scope.canManageAccount = AccessService.canManageAccount();
      $scope.canManageeMenu = AccessService.canManageeMenu();
    }

    $scope.displayMessage = function(type, message) {
      toaster.pop(type, null, message);
    };
  }]);
