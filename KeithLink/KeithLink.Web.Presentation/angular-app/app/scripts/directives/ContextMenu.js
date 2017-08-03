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
            listHeaders = ListService.listHeaders;

        OrderService.getChangeOrders().then(function(resp){
          $scope.changeOrders = resp;
        });

        $scope.lists = listHeaders.length > 0 ? listHeaders : ListService.getListHeaders();

        if ($scope.canCreateOrders) {
          CartService.getShipDates(); // needed if user creates a cart using the context menu

        $scope.carts = cartHeaders.length > 0 ? cartHeaders : CartService.getCartHeaders();

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

      $scope.addItemToList = function(list, item, selectedList) {
      var newItem = angular.copy(item);
        if(selectedList && selectedList.iscustominventory){
          var newItem = [
            item
          ];
          $q.all([
            ListService.addNewItemsFromCustomInventoryList(list, newItem),
          ]).then(function(data) {
            item.favorite = true;
            closeModal();
          });
        } else {
          $q.all([
            ListService.addItem(list, newItem),
            ListService.addItemToFavorites(newItem)
          ]).then(function(data) {
            item.favorite = true;
            closeModal();
            $scope.displayMessage('success', 'Successfully added item to list ' + list.name + '.');
          }, function() {
            $scope.displayMessage('error', 'Error adding item to list ' + list.name + '.');
          });
        }
      };

      $scope.createListWithItem = function(item) {
        var newListItem = item;
        $q.all([
          ListService.createList(newListItem),
          ListService.addItemToFavorites(newListItem)
        ]).then(function(data) {
          ListService.getListHeaders().then(function(listheaders){
            $rootScope.listHeaders = listheaders;
          })
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
