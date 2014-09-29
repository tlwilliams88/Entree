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

    $scope.branches = branches;
    refreshAccessPermissions();
    
    // for guest users
    $scope.changeBranch = function() {
      UserProfileService.setBranchId($scope.currentLocation);
    }
    // for order-entry customers
    $scope.changeCustomerLocation = function() {
      UserProfileService.setBranchId($scope.currentLocation.customerBranch);
      UserProfileService.setCustomerNumber($scope.currentLocation.customerNumber);
    };

    if ($scope.userProfile.user_customers) {
      $scope.currentLocation = $scope.userProfile.user_customers[0];
      $scope.changeCustomerLocation();
    } else {
      $scope.currentLocation = UserProfileService.profile().branchid;
      $scope.changeBranch();
    }

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
