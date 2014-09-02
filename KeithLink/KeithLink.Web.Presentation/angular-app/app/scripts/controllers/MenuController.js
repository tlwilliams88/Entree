'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MenuController', ['$scope', '$state', '$modal', 'Constants', 'AuthenticationService', 'UserProfileService', 'AccessService', 
    function ($scope, $state, $modal, Constants, AuthenticationService, UserProfileService, AccessService) {

    $scope.userProfile = UserProfileService.profile();
    $scope.userBar = {};
    $scope.userBar.universalSearchTerm = '';

    refreshAccessPermissions();

    if (AccessService.isLoggedIn()) {
      setLocations(UserProfileService.profile());
    }

    $scope.logout = function() {
      AuthenticationService.logout();
      refreshAccessPermissions();

      $state.transitionTo('register');
      $scope.displayUserMenu = false;
    };

    $scope.changeLocation = function() {
      UserProfileService.setCurrentLocation($scope.currentLocation);
    };

    $scope.print = function () {
      window.print(); 
    };

    function setLocations(profile) {
      if (AccessService.isOrderEntryCustomer()) {
        // branches will the branches the user has access to, this will come back in the profile
        $scope.locations = profile.stores;
        var currentLocation = profile.stores[0];
        $scope.currentLocation = currentLocation;
      } else {
        // branches will the full list of branches with the user's default branch selected
        setDefaultLocations();
        $scope.currentLocation =  { 'name': 'San Antonio', 'branchId': 'fsa' }; // default location
      }

      // set the user's current location from cache if available
      if (UserProfileService.getCurrentLocation()) {
        $scope.currentLocation = UserProfileService.getCurrentLocation();
      }

      UserProfileService.setCurrentLocation($scope.currentLocation);
    }

    function setDefaultLocations() {
      $scope.locations = [{
        'name': 'Dallas Ft Worth',
        'branchId': 'fdf'
      }, {
        'name': 'San Antonio',
        'branchId': 'fsa'
      }, {
        'name': 'Amarillo',
        'branchId': 'fam'
      }];
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
    
    $scope.menuItems = [
      {
        'text': 'Home',
        'icon': 'house',
        'sref': 'menu.home',
        'accessRule': 'isOrderEntryCustomer'
      }, {
        'text': 'Order History',
        'icon': 'clipboard',
        'sref': 'blank',
        'accessRule': 'canCreateOrders'
      }, {
        'text': 'Product Catalog',
        'icon': 'book2',
        'sref': 'menu.catalog.home',
        'accessRule': 'canBrowseCatalog'
      }, {
        'text': 'Reports',
        'icon': 'pie',
        'sref': 'blank',
        'accessRule': 'canPayInvoices'
      }, {
        'text': 'Invoices',
        'icon': 'dollar',
        'sref': 'blank',
        'accessRule': 'canPayInvoices'
      }, {
        'text': 'eMenuManage',
        'icon': 'food',
        'sref': 'blank',
        'accessRule': 'canManageeMenu'
      }, {
        'text': 'My Lists',
        'icon': 'text',
        'sref': 'menu.lists',
        'accessRule': 'canManageLists'
      }, {
        'text': 'Add to Order',
        'icon': 'add-to-list',
        'sref': 'blank',
        'accessRule': 'canCreateOrders'
      }, {
        'text': 'Notifications',
        'icon': 'bell',
        'sref': 'blank',
        'accessRule': 'isOrderEntryCustomer',
        'isHiddenDesktop': true
      }
    ];

  }]);
