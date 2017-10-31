'use strict';

angular.module('bekApp')
.directive('orderDropdown', [function(){
  return {
    restrict: 'E',
    // replace : true,
    scope: {
      openScope: '=',
      isDisabled: '='
    },
    templateUrl: 'views/directives/orderdropdown.html',
    controller: ['$scope', '$stateParams', '$modal', 'OrderService', '$state', 'ApplicationSettingsService', 'UtilityService', 'LocalStorage', 'ListService', 'CartService',
     function($scope, $stateParams, $modal, OrderService, $state, ApplicationSettingsService, UtilityService, LocalStorage, ListService, CartService){

      $scope.isHomePage = $stateParams.isHomePage;
      $scope.isMobile = UtilityService.isMobileDevice();
      var currentCustomer = LocalStorage.getCurrentCustomer(),
          shipDates = CartService.getShipDates(),
          cartHeaders = CartService.cartHeaders ? CartService.cartHeaders : CartService.getCartHeaders(),
          listHeaders = ListService.listHeaders.length > 0 ? ListService.listHeaders : ListService.getListHeaders(),
          isOffline = CartService.isOffline,
          selectedList,
          customListHeaders;

      if($scope.isMobile){
        customListHeaders = false;
      } else {
        customListHeaders = ListService.getCustomListHeaders();
      }

      if ($scope.isDisabled) {
        $scope.tooltipMessage = 'Customer is not set up for ordering in the Entrée System.';
      }

      $scope.openCreateOrderModal = function(size) {
        selectedList = ApplicationSettingsService.getDefaultOrderList();
        var modalInstance = $modal.open({
          templateUrl: 'views/modals/createordermodal.html',
          controller: 'CreateOrderModalController',
          backdrop:'static',
          size: size,
          resolve: {
            CurrentCustomer: function() {
              return currentCustomer;
            },
            ShipDates: function() {
              return shipDates;
            },
            CartHeaders: function() {
              return cartHeaders;
            },
            Lists: function() {
              return ListService.getListHeaders();
            },
            Orders: function() {
              return OrderService.getOrders({
                      from: 0,
                      size: 6,
                      sort:  [{
                        field: 'createddate',
                        order: 'desc'
                      }]
                    });
            },
            CustomListHeaders: function() {
              return customListHeaders;
            },
            IsOffline: function() {
              return isOffline;
            },
            IsMobile: function() {
              return $scope.isMobile;
            },
            SelectedList: function() {
              return selectedList;
            }
          }
        });

        modalInstance.result.then(function(cart) {
          if(cart.valid){
            if(cart.type && cart.type == 'QuickAdd'){
              $state.go('menu.cart.items', {cartId: cart.id});
            } else if(cart.type && cart.type == 'Import') {
              $state.go('menu.cart.items', { cartId: cart.listid });
            } else {
              $state.go('menu.addtoorder.items', { listId: cart.listId, listType: cart.listType, cartId: cart.id});
            }
          }
        });
      };

      $scope.openQuickAddModal = function() {
        var modalInstance = $modal.open({
          templateUrl: 'views/modals/cartquickaddmodal.html',
          controller: 'CartQuickAddModalController',
          backdrop:'static',

          resolve: {
            cart: function() {
              return;
            }
          }
        });

        modalInstance.result.then(function(cartId) {
          $state.go('menu.cart.items', {cartId: cartId});
        });
      };

      $scope.openOrderImportModal = function () {
        var modalInstance = $modal.open({
          templateUrl: 'views/modals/orderimportmodal.html',
          controller: 'ImportModalController',
          resolve: {
            customListHeaders: ['ListService', function(ListService) {
              return ListService.getCustomListHeaders();
            }]
          }
        });
      };
    }]
  };
}]);
