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
    $scope.userBar = {};
    $scope.userBar.universalSearchTerm = '';

    $scope.userProfile = LocalStorage.getProfile();
    $scope.branches = branches;
    refreshAccessPermissions();
    $scope.userBar.userNotificationsCount = NotificationService.userNotificationsCount;

    $scope.selectedUserContext = LocalStorage.getCurrentCustomer();
    $scope.customerInfiniteScroll = {
      from: 0,
      size: 15
    };
    $scope.customerSelectOptions = {
      query: function (query){
        $scope.customerInfiniteScroll.from = (query.page - 1) * $scope.customerInfiniteScroll.size;

        UserProfileService.searchUserCustomers(query.term, $scope.customerInfiniteScroll.size, $scope.customerInfiniteScroll.from).then(function(data) {
          var obj = {
            results: [],
            more: query.page * 15 < data.totalResults
          };
          data.results.forEach(function(customer) {
            obj.results.push({
              id: customer.customerNumber,
              text: customer.displayname,
              customer: customer
            });
          });
          query.callback(obj);
        });
      }
    };

    // for guest users
    $scope.changeBranch = function() {
      LocalStorage.setSelectedBranchInfo($scope.selectedUserContext);

      $state.transitionTo($state.current, $state.params, {
        reload: true,
        inherit: false,
        notify: true
      });
    };

    // for order-entry customers
    $scope.changeCustomerLocation = function(selectedUserContext) {
      LocalStorage.setSelectedCustomerInfo(selectedUserContext);

      angular.copy([], CartService.shipDates);

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

      $scope.backfunction = function(){
        console.log('back');
        $window.history.back();
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

      // $scope.isInternalUser = AccessService.isInternalUser();
      // $scope.isBekAdmin = AccessService.isBekAdmin();

      $scope.isDsr = AccessService.isDsr();

      $scope.canBrowseCatalog = AccessService.canBrowseCatalog();
      $scope.canManageLists = AccessService.canManageLists();
      $scope.canCreateOrders = AccessService.canCreateOrders();
      $scope.canSubmitOrders = AccessService.canSubmitOrders();
      $scope.canPayInvoices = AccessService.canPayInvoices();
      $scope.canManageAccount = AccessService.canManageAccount();


    }
  }]);
