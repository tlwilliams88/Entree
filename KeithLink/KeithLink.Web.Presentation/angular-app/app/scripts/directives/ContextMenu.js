'use strict';

angular.module('bekApp')
.directive('contextMenu', [ function(){
  return {
    restrict: 'A',
    scope: true,
    transclude: true,
    controller: ['$scope', '$state', '$q', '$modal', 'toaster', 'ListService', 'CartService', 'OrderService',
    function($scope, $state, $q, $modal, toaster, ListService, CartService, OrderService){

      /*************
      LISTS
      *************/

      $scope.addItemToList = function(listId, item) {
        var newItem = angular.copy(item);
        
        $q.all([
          ListService.addItem(listId, item),
          ListService.addItemToFavorites(item)
        ]).then(function(data) {
          item.favorite = true;
          $scope.isContextMenuDisplayed = false;
          $scope.displayMessage('success', 'Successfully added item to list.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to list.');
        });
      };

      $scope.addItemToReminderList = function(item) {
        ListService.addItemToListWithoutDuplicates(item).then(function(data) {
          $scope.displayMessage('success', 'Successfully added item to Reminder List.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to Reminder List.');
        });
      };

      $scope.createListWithItem = function(item) {
        $q.all([
          ListService.createList(item),
          ListService.addItemToFavorites(item)
        ]).then(function(data) {
          $state.go('menu.lists.items', { listId: data[0].listid, renameList: true });
          $scope.displayMessage('success', 'Successfully created new list.');
        }, function() {
          $scope.displayMessage('error', 'Error creating new list.');
        });
      };

      /*************
      CARTS
      *************/

      $scope.addItemToCart = function(cartId, item) {
        CartService.addItemToCart(cartId, item).then(function(data) {
          $scope.isContextMenuDisplayed = false;
          $scope.displayMessage('success', 'Successfully added item to cart.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to cart.');
        });
      };

      $scope.createCartWithItem = function(item) {
        var items = [item];
        CartService.createCart(items).then(function(data) {
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
          itemnumber: item.itemnumber
        };
        order.lineItems.push(orderItem);

        OrderService.updateOrder(order).then(function(data) {
          $scope.isContextMenuDisplayed = false;
          $scope.displayMessage('success', 'Successfully added item to Order #' + order.ordernumber + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to Order #' + order.ordernumber + '.');
        });
      };

      /*************
      OPEN BEHAVIOR
      *************/

      $scope.closeContextMenu = function() {
        $scope.isContextMenuDisplayed = false;
      };

      function isTouchDevice() {
        return ('ontouchstart' in window) || navigator.maxTouchPoints > 0 || navigator.msMaxTouchPoints > 0 || window.innerWidth <= 991;
      }

      $scope.openContextMenu = function (e, item, lists, carts, changeOrders) {
        if (isTouchDevice()) {
          var modalInstance = $modal.open({
            templateUrl: 'views/contextmenumodal.html',
            controller: 'ContextMenuModalController',
            // backdrop: false,
            resolve: {
              lists: function () {
                return lists;
              },
              carts: function () {
                return carts;
              },
              item: function() {
                return item;
              },
              changeOrders: function() {
                return changeOrders;
              }
            }
          });
        } else {
          $scope.isContextMenuDisplayed = true;
        }
      };
    }],
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);