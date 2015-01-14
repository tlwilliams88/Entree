'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MenuController', ['$scope', '$state', '$modal', '$window', 'branches', 'UserProfileService', 'AuthenticationService', 'AccessService', 'LocalStorage', 'CartService', 'NotificationService',
    function ($scope, $state, $modal, $window, branches, UserProfileService, AuthenticationService, AccessService, LocalStorage, CartService, NotificationService) {

  $scope.$state = $state;

  // define search term in user bar so it can be cleared in the SearchController after a user searches
  $scope.userBar = {};
  $scope.userBar.universalSearchTerm = '';
  $scope.branches = branches;

  // global notification at the top of all pages
  // TODO: Global messaging backend?
  $scope.messageText = 'Hello world!';
  $scope.displayGlobalMessage = true;

  $scope.userProfile = LocalStorage.getProfile();
  refreshAccessPermissions();
  $scope.userBar.userNotificationsCount = NotificationService.userNotificationsCount;
 
  /**********
  SELECTED USER CONTEXT
  **********/

  $scope.setSelectedUserContext = function(selectedUserContext) {
    $scope.selectedUserContext = selectedUserContext;
  };

  // get default selected user context
  if ($scope.isOrderEntryCustomer) {
    $scope.setSelectedUserContext(LocalStorage.getCurrentCustomer());
  } else {
    $scope.setSelectedUserContext(LocalStorage.getBranchId());
  }

  $scope.customerInfiniteScroll = {
    from: 0,
    size: 15
  };

  // populates upper-left customer dropdown infinite scroll
  $scope.customerSelectOptions = {
    query: function (query){
      $scope.customerInfiniteScroll.from = (query.page - 1) * $scope.customerInfiniteScroll.size;

      UserProfileService.searchUserCustomers(query.term, $scope.customerInfiniteScroll.size, $scope.customerInfiniteScroll.from).then(function(data) {
        // convert data to match select2 data object
        var obj = {
          results: [],
          more: query.page * 15 < data.totalResults // boolean if there are more results to display using infinite scroll
        };

        data.results.forEach(function(customer) {
          obj.results.push({
            id: customer.customerNumber, // value
            text: customer.displayname,  // display text
            customer: customer
          });
        });
        query.callback(obj);
      });
    }
  };

  $scope.goToAdminLandingPage = function() {
    // internal bek admin user
    if (AccessService.isInternalUser()) {
      $state.go('menu.admin.customergroup');
      
    // external owner admin
    } else {  
      $state.go('menu.admin.customergroupdashboard');
    }
  };

  function refreshPage() {
    $state.transitionTo($state.current, $state.params, {
      reload: true,
      inherit: false,
      notify: true
    });
  }

  // change context menu selection for guest users
  $scope.changeBranch = function() {
    LocalStorage.setSelectedBranchInfo($scope.selectedUserContext);
    refreshPage();
  };

  // change context menu selection for order-entry customers
  $scope.changeCustomerLocation = function(selectedUserContext) {
    LocalStorage.setSelectedCustomerInfo(selectedUserContext);

    // reset ship dates in cache
    angular.copy([], CartService.shipDates);

    refreshPage();
  };

  /**********
  MENU BUTTON CLICK HANDLERS
  **********/

  $scope.logout = function() {
    AuthenticationService.logout();

    $state.transitionTo('register');
    $scope.displayUserMenu = false;
  };

  $scope.print = function () {
    $window.print();
  };

  $scope.navigateBack = function(){
    $window.history.back();
  };

  $scope.openTechnicalSupportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/technicalsupportmodal.html',
      controller: 'TechnicalSupportModalController',
      windowClass: 'color-background-modal',
      resolve: {
        branch: function() {
          var branchFound,
            branchId = LocalStorage.getBranchId();
          angular.forEach(branches, function(branch) {
            if (branch.id.toUpperCase() === branchId.toUpperCase()) {
              branchFound = branch;
            }
          });
          return branchFound;
        }
      }
    });
  };

  /**********
  SET USER PERMISSIONS
  these are used in all sub-states to show/hide accessible info
  also used in state.js to determine if the user has access to a state
  **********/

  function refreshAccessPermissions() {
    $scope.isLoggedIn = AccessService.isLoggedIn();
    $scope.isOrderEntryCustomer = AccessService.isOrderEntryCustomer();

    // $scope.isInternalUser = AccessService.isInternalUser();
    // $scope.isBekAdmin = AccessService.isBekAdmin();

    $scope.isDsr = AccessService.isDsr();

    $scope.canBrowseCatalog = AccessService.canBrowseCatalog();
    $scope.canSeePrices = AccessService.canSeePrices();
    $scope.canManageLists = AccessService.canManageLists();
    $scope.canCreateOrders = AccessService.canCreateOrders();
    $scope.canSubmitOrders = AccessService.canSubmitOrders();
    $scope.canPayInvoices = AccessService.canPayInvoices();
    $scope.canManageAccount = AccessService.canManageAccount();
    $scope.canManageAccounts = AccessService.canManageAccounts();


  }
}]);
