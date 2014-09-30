'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MenuController', ['$scope', '$state', '$modal', 'branches', 'toaster', 'AuthenticationService', 'AccessService', 'LocalStorage',
    function ($scope, $state, $modal, branches, toaster, AuthenticationService, AccessService, LocalStorage) {

    $scope.$state = $state;

    $scope.userProfile = LocalStorage.getProfile();
    $scope.userBar = {};
    $scope.userBar.universalSearchTerm = '';
    $scope.currentLocation = LocalStorage.getCurrentLocation();

    $scope.branches = branches;
    refreshAccessPermissions();
    
    // for guest users
    $scope.changeBranch = function() {
      LocalStorage.setBranchId($scope.currentLocation);
      LocalStorage.setCurrentLocation($scope.currentLocation);
    }
    // for order-entry customers
    $scope.changeCustomerLocation = function() {
      LocalStorage.setBranchId($scope.currentLocation.customerBranch);
      LocalStorage.setCustomerNumber($scope.currentLocation.customerNumber);
      LocalStorage.setCurrentLocation($scope.currentLocation);
    };

    $scope.logout = function() {
      AuthenticationService.logout();
      // refreshAccessPermissions();

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
