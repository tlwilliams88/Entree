'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MenuController', ['$scope', '$state', '$modal', 'branches', 'Constants', 'AuthenticationService', 'UserProfileService', 'AccessService', 
    function ($scope, $state, $modal, branches, Constants, AuthenticationService, UserProfileService, AccessService) {

    $scope.$state = $state;

    $scope.userProfile = UserProfileService.profile();
    $scope.userBar = {};
    $scope.userBar.universalSearchTerm = '';

    refreshAccessPermissions();
    // setLocations(UserProfileService.profile());

    $scope.changeLocation = function() {
      UserProfileService.setCurrentLocation($scope.currentLocation);
    };

    $scope.branches = branches;
    $scope.currentLocation = UserProfileService.profile().branchId;
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

    // function setLocations(profile) {
    //   if ($scope.isOrderEntryCustomer) {
    //     $scope.locations = profile.stores;
    //   } else {
    //     $scope.locations = branches;
    //   }

    //   $scope.currentLocation = UserProfileService.profile().branchId;
    //   if (!$scope.currentLocation) {
    //     $scope.currentLocation = $scope.locations[0];
    //   }
    // }

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
