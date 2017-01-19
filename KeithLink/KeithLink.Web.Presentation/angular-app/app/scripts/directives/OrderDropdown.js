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
    controller: ['$scope', '$stateParams', '$modal', '$state', 'ApplicationSettingsService', 'UtilityService', 'LocalStorage', 'ListService', 'CartService', function($scope, $stateParams, $modal, $state, ApplicationSettingsService, UtilityService, LocalStorage, ListService, CartService){

      $scope.isHomePage = $stateParams.isHomePage;
      $scope.isMobile = UtilityService.isMobileDevice();
      var currentCustomer = LocalStorage.getCurrentCustomer(),
          shipDates = CartService.getShipDates(),
          cartHeaders = CartService.cartHeaders ? CartService.cartHeaders : CartService.getCartHeaders(),
          listHeaders = ListService.getListHeaders(),
          selectedList= ApplicationSettingsService.getDefaultOrderList(),
          isOffline = CartService.isOffline,
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
              return listHeaders;
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
          if(cart.type && cart.type == 'QuickAdd'){
            $state.go('menu.cart.items', {cartId: cart.id});
          } else if(cart.type && cart.type == 'Import') {
            $state.go('menu.cart.items', { cartId: cart.listid });
          } else {
            $state.go('menu.addtoorder.items', { listId: cart.listid, cartId: cart.id});
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