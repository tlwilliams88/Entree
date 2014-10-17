'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MenuController', ['$scope', '$state', '$stateParams', '$modal', 'branches', 'AuthenticationService', 'AccessService', 'LocalStorage',
    function ($scope, $state, $stateParams, $modal, branches, AuthenticationService, AccessService, LocalStorage) {

    $scope.$state = $state;
    $scope.userBar = {};
    $scope.userBar.universalSearchTerm = '';

    $scope.userProfile = LocalStorage.getProfile();
    $scope.branches = branches;
    refreshAccessPermissions();

    // if ($scope.isOrderEntryCustomer) {
    //   $scope.currentLocation = LocalStorage.getCurrentLocation().customerNumber;
    // } else {
      $scope.currentLocation = LocalStorage.getCurrentLocation();
    // }
    
    // for guest users
    $scope.changeBranch = function() {
      LocalStorage.setBranchId($scope.currentLocation);
      LocalStorage.setCurrentLocation($scope.currentLocation);
    };
    // for order-entry customers
    $scope.changeCustomerLocation = function() {
      angular.forEach($scope.userProfile.user_customers, function(customer) {
        if (customer.customerNumber === $scope.currentLocation) {
          LocalStorage.setBranchId(customer.customerBranch);
          LocalStorage.setCustomerNumber(customer.customerNumber);
          LocalStorage.setCurrentLocation(customer.customerNumber);
        }
      }); 
      
      $state.transitionTo($state.current, $state.params, {
        reload: true,
        inherit: false,
        notify: true
      });
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
  }]);
