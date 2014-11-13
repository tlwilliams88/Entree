'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MenuController', ['$scope', '$state', '$modal', 'branches', 'AuthenticationService', 'AccessService', 'LocalStorage', 'CartService', 'NotificationService',
    function ($scope, $state, $modal, branches, AuthenticationService, AccessService, LocalStorage, CartService, NotificationService) {

    $scope.$state = $state;
    $scope.userBar = {};
    $scope.userBar.universalSearchTerm = '';

    $scope.userProfile = LocalStorage.getProfile();
    $scope.branches = branches;
    refreshAccessPermissions();
    $scope.currentLocation = LocalStorage.getCurrentLocation();

    $scope.userBar.userNotifications = NotificationService.userNotifications;

    // for guest users
    $scope.changeBranch = function() {
      LocalStorage.setBranchId($scope.currentLocation);
      LocalStorage.setCurrentLocation($scope.currentLocation);

      $state.transitionTo($state.current, $state.params, {
        reload: true,
        inherit: false,
        notify: true
      });
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

      angular.copy([], CartService.shipDates);
      // CartService.getShipDates();
      
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

    $scope.openTechnicalSupportModal = function() {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/technicalsupportmodal.html',
        controller: 'TechnicalSupportModalController',
        windowClass: 'color-background-modal',
        resolve: {
          branchId: function() {
            return LocalStorage.getBranchId();
          },
          branches: function() {
            return branches;
          }
        }
      });
    };

    function refreshAccessPermissions() {
      $scope.isLoggedIn = AccessService.isLoggedIn();
      $scope.isOrderEntryCustomer = AccessService.isOrderEntryCustomer();
      
      $scope.isBekAdmin = AccessService.isBekAdmin();
      $scope.isCustomerAdmin = AccessService.isCustomerAdmin();

      $scope.canBrowseCatalog = AccessService.canBrowseCatalog();
      $scope.canManageLists = AccessService.canManageLists();
      $scope.canCreateOrders = AccessService.canCreateOrders();
      $scope.canSubmitOrders = AccessService.canSubmitOrders();
      $scope.canPayInvoices = AccessService.canPayInvoices();
      $scope.canManageAccount = AccessService.canManageAccount();
      $scope.canManageeMenu = AccessService.canManageeMenu();
    }
  }]);
