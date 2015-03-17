'use strict';

/*
to be used in conjunction with contextMenuTemplate directive
*/

angular.module('bekApp')
.directive('contextMenu', [ function(){
  return {
    restrict: 'A',
    // scope: true,
    controller: ['$scope', '$state', '$q', '$modal', 'toaster', 'ListService', 'CartService', 'OrderService',
    function($scope, $state, $q, $modal, toaster, ListService, CartService, OrderService){

      ListService.getListHeaders().then(function(lists) {
        $scope.lists = lists;
      });

      CartService.getCartHeaders().then(function(carts) {
        $scope.carts = carts;
      });

      OrderService.getChangeOrders().then(function(orders) {
        $scope.changeOrders = orders;
      });

      /*************
      LISTS
      *************/

      $scope.addItemToList = function(listId, item) {
        $q.all([
          ListService.addItem(listId, item),
          ListService.addItemToFavorites(item)
        ]).then(function(data) {
          item.favorite = true;
          $scope.$broadcast('closeContextMenu');
        });
      };

      $scope.createListWithItem = function(item) {
        $q.all([
          ListService.createList(item),
          ListService.addItemToFavorites(item)
        ]).then(function(data) {
          $scope.$broadcast('closeContextMenu');
          $state.go('menu.lists.items', { listId: data[0].listid, renameList: true });
        });
      };

      /*************
      CARTS
      *************/

      $scope.addItemToCart = function(cartId, item) {
        CartService.addItemToCart(cartId, item).then(function(data) {
          $scope.$broadcast('closeContextMenu');
          item.quantityincart += 1;
          $scope.displayMessage('success', 'Successfully added item to cart.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to cart.');
        });
      };

      $scope.createCartWithItem = function(item) {
        var items = [item];
        CartService.createCart(items).then(function(data) {
          $scope.$broadcast('closeContextMenu');
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

        OrderService.updateOrder(order).then(function(data) {
          $scope.$broadcast('closeContextMenu');
          $scope.displayMessage('success', 'Successfully added item to Order #' + order.invoicenumber + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to Order #' + order.invoicenumber + '.');
        });
      };

    }]
  };
}]);