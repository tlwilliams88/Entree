'use strict';

/*
to be used in conjunction with contextMenuTemplate directive
*/

angular.module('bekApp')
.directive('contextMenu', [ function(){
  return {
    restrict: 'A',
    // scope: true,
    controller: ['$scope', '$rootScope', '$state', '$q', '$modal', 'toaster', 'ListService', 'CartService', 'OrderService', 'ContextMenuService',
    function($scope, $rootScope, $state, $q, $modal, toaster, ListService, CartService, OrderService, ContextMenuService){

      if ($scope.isOrderEntryCustomer) {
        var cartHeaders = CartService.cartHeaders,
            listHeaders = ListService.listHeaders,
            changeOrderHeaders = OrderService.changeOrderHeaders;

        if(listHeaders.length){
          $scope.lists = ListService.listHeaders;
        }

        if ($scope.canCreateOrders) {
          CartService.getShipDates(); // needed if user creates a cart using the context menu

        $scope.carts = cartHeaders.length ? cartHeaders : CartService.getCartHeaders();
        $scope.changeOrders = changeOrderHeaders.length ? changeOrderHeaders : OrderService.getChangeOrders();

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

      $scope.addItemToList = function(listName, listId, item, selectedList) {
      var newItem = angular.copy(item);
        if(selectedList && selectedList.iscustominventory){
          var newItem = [
            item
          ];
          $q.all([
            ListService.addNewItemsFromCustomInventoryList(listId, newItem),
          ]).then(function(data) {
            item.favorite = true;
            closeModal();
          });
        } else {
          $q.all([
            ListService.addItem(listId, newItem),
            ListService.addItemToFavorites(newItem)
          ]).then(function(data) {
            item.favorite = true;
            closeModal();
            $scope.displayMessage('success', 'Successfully added item to list ' + listName + '.');
          }, function() {
            $scope.displayMessage('error', 'Error adding item to list ' + listName + '.');
          });
        }
      };

      $scope.createListWithItem = function(item) {
        $q.all([
          ListService.createList(item),
          ListService.addItemToFavorites(item)
        ]).then(function(data) {
          $rootScope.$broadcast('ListCreatedFromContextMenu');
          closeModal();
        });
      };

      /*************
      CARTS
      *************/

      $scope.addItemToCart = function(cartName, cartId, item) {
        var newItem = angular.copy(item);
        CartService.addItemToCart(cartId, newItem).then(function(data) {
          closeModal();
          $scope.displayMessage('success', 'Successfully added item to cart ' + cartName + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to cart ' + cartName + '.');
        });
      };

      $scope.createCartWithItem = function(item) {
        var items = [item];
        CartService.renameCart = true;
        CartService.createCart(items).then(function(data) {
          closeModal();
          $scope.displayMessage('success', 'Successfully created new cart ' + data.name + '.');
        }, function() {
          $scope.displayMessage('error', 'Error creating new cart.');
        });
      };

      /*************
      CHANGE ORDERS
      *************/

       $scope.addItemToChangeOrder = function(order, item) {
        var orderItem = angular.copy(item);
        orderItem.quantity = (orderItem.quantity && orderItem.quantity > 0) ? orderItem.quantity : 1;
        orderItem.each = (orderItem.each) ? true : false;
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
