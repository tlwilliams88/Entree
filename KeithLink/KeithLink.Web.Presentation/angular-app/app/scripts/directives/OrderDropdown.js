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
    controller: ['$scope', '$modal', '$state', 'UtilityService', function($scope, $modal, $state, UtilityService){

      $scope.isMobile = UtilityService.isMobileDevice();

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
            CurrentCustomer: ['LocalStorage', function(LocalStorage) {
              return LocalStorage.getCurrentCustomer();
            }],
            ShipDates: ['CartService', function(CartService) {
              return CartService.getShipDates();
            }],
            CartHeaders: ['CartService', function(CartService) {
              return CartService.getCartHeaders();
            }],
            Lists: ['ListService', function(ListService) {
              return ListService.getListHeaders();
            }],
            CustomListHeaders: ['ListService', function(ListService) {
              return ListService.getCustomListHeaders();
            }],
            isMobile: function() {
              return $scope.isMobile;
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