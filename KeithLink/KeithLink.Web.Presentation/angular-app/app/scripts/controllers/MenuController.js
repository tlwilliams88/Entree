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
    setLocations(UserProfileService.profile());

    $scope.logout = function() {
      AuthenticationService.logout();
      refreshAccessPermissions();

      $state.transitionTo('register');
      $scope.displayUserMenu = false;
    };

    $scope.changeLocation = function() {
      UserProfileService.setCurrentLocation($scope.currentLocation);
    };

    $scope.search = function(searchTerm) {
      $state.go('menu.catalog.products.list', { type: 'search', id: searchTerm }, { reload: true });
    };

    $scope.print = function () {
      window.print(); 
    };

    function setLocations(profile) {
      $scope.locations = profile.stores;
      $scope.currentLocation = UserProfileService.getCurrentLocation();

      UserProfileService.setCurrentLocation($scope.currentLocation);
    }

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
