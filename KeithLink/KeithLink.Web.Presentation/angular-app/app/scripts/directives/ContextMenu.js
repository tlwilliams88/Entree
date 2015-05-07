'use strict';

/*
to be used in conjunction with contextMenuTemplate directive
*/

angular.module('bekApp')
.directive('contextMenu', [ function(){
  return {
    restrict: 'A',
    // scope: true,
    controller: ['$scope', '$state', '$q', '$modal', 'toaster', 'ListService', 'CartService', 'OrderService', 'ContextMenuService',
    function($scope, $state, $q, $modal, toaster, ListService, CartService, OrderService, ContextMenuService){

      if ($scope.isOrderEntryCustomer) {

        ListService.getListHeaders().then(function(lists) {
          $scope.lists = lists;
        });

        if ($scope.canCreateOrders) {
          CartService.getShipDates(); // needed if user creates a cart using the context menu
          
          CartService.getCartHeaders().then(function(carts) {
            $scope.carts = carts;
          });

          OrderService.getChangeOrders().then(function(orders) {
            $scope.changeOrders = orders;
          });
        }
      }

      function closeModal() {
        $scope.$broadcast('closeContextMenu');

        // if (ContextMenuService.menuElement) {
        //   ContextMenuService.menuElement.removeClass('open');
        // }

        if (ContextMenuService.modalElement) {
          ContextMenuService.modalElement.close();
        }
      }

      /*************
      LISTS
      *************/

      $scope.addItemToList = function(listId, item) {
      var newItem = angular.copy(item);
        $q.all([
          ListService.addItem(listId, newItem),
          ListService.addItemToFavorites(newItem)
        ]).then(function(data) {
          item.favorite = true;
          closeModal();
        });
      };

      $scope.createListWithItem = function(item) {
        $q.all([
          ListService.createList(item),
          ListService.addItemToFavorites(item)
        ]).then(function(data) {
          closeModal();
          $state.go('menu.lists.items', { listId: data[0].listid, renameList: true });
        });
      };

      /*************
      CARTS
      *************/

      $scope.addItemToCart = function(cartId, item) {
        var newItem = angular.copy(item);
        CartService.addItemToCart(cartId, newItem).then(function(data) {
          closeModal();
          item.quantityincart += 1;
          $scope.displayMessage('success', 'Successfully added item to cart.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to cart.');
        });
      };

      $scope.createCartWithItem = function(item) {
        var items = [item];
        CartService.renameCart = true;
        CartService.createCart(items).then(function(data) {
          closeModal();
          $state.go('menu.cart.items', { cartId: data.id, renameCart: true });
          $scope.displayMessage('success', 'Successfully created new cart.');
        }, function() {
          $scope.displayMessage('error', 'Error creating new cart.');
        });
      };

      /*************
      CHANGE ORDERS
      *************/

       $scope.addItemToChangeOrder = function(order, item) {
        var orderItem = {
          quantity: 1,
          itemnumber: item.itemnumber,
          each: item.each
        };
        order.items.push(orderItem);

        var params = {
          deleteOmitted: false
        };

        OrderService.updateOrder(order, params).then(function(data) {
          closeModal();
          $scope.displayMessage('success', 'Successfully added item to Order #' + order.invoicenumber + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to Order #' + order.invoicenumber + '.');
        });
      };

    }]
  };
}]);