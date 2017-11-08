'use strict';

/*
to be used in conjunction with contextMenuTemplate directive
*/

angular.module('bekApp')
.directive('contextMenu', [ function(){
  return {
    restrict: 'A',
    // scope: true,
    controller: ['$scope', '$rootScope', '$state', '$q', '$modal', 'toaster', 'ListService', 'CartService', 'OrderService', 'ContextMenuService', '$filter',
    function($scope, $rootScope, $state, $q, $modal, toaster, ListService, CartService, OrderService, ContextMenuService, $filter){

      if ($scope.isOrderEntryCustomer) {
        var cartHeaders = CartService.cartHeaders,
            listHeaders = ListService.listHeaders,
            changeOrderHeaders = OrderService.changeOrderHeaders;

      if(changeOrderHeaders == null || changeOrderHeaders.length == 0) {
          OrderService.getChangeOrders().then(function(resp){
            $scope.changeOrders = resp;
          });
      }

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

      $scope.addItemToList = function(item, selectedList) {
      var newItem = angular.copy(item);
        if(selectedList && selectedList.iscustominventory){
          var newItem = [
            item
          ];
          $q.all([
            ListService.addNewItemsFromCustomInventoryList(selectedList, newItem),
          ]).then(function(data) {
            item.favorite = true;
            closeModal();
          });
        } else {
          closeModal();

          if(item.favorite == false) {
              item.favorite = true;              
              $q.all([
                ListService.addItem(selectedList, newItem),
                ListService.addItemToFavorites(newItem)
              ]);
          } else {
              ListService.addItemToFavorites(newItem);
          }
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
        
        var contractList = $filter('filter')($scope.lists, { type: 2 }),
            historyList = $filter('filter')($scope.lists, { type: 5 }),
            favoritesList = $filter('filter')($scope.lists, { type: 1 }),
            customList = $filter('filter')($scope.lists, { type: 0 }),
            listToBeUsed = {};

        if(contractList.length > 0){
          listToBeUsed = contractList[0];
        }
        else if(historyList.length > 0){
          listToBeUsed = historyList[0];
        }
        else if(favoritesList.length > 0){
          listToBeUsed = favoritesList[0];
        }
        else if(customList.length > 0){
          listToBeUsed = customList[0];
        }
        
        CartService.createCart(items, null, null, null, listToBeUsed.listid, listToBeUsed.type).then(function(data) {
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
