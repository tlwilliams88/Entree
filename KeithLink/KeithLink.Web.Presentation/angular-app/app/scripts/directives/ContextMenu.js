'use strict';

angular.module('bekApp')
.directive('contextMenu', [ function(){
  return {
    restrict: 'A',
    scope: true,
    transclude: true,
    controller: ['$scope', '$state', '$q', '$modal', 'toaster', 'ListService', 'CartService', 
    function($scope, $state, $q, $modal, toaster, ListService, CartService){

      $scope.addItemToList = function(listId, item) {
        var newItem = angular.copy(item);
        
        $q.all([
          ListService.addItem(listId, item),
          ListService.addItemToFavorites(item)
        ]).then(function(data) {
          item.favorite = true;
          $scope.displayedItems.isContextMenuDisplayed = false;
          $scope.displayMessage('success', 'Successfully added item to list.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to list.');
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

      $scope.addItemToCart = function(cartId, item) {
        CartService.addItemToCart(cartId, item).then(function(data) {
          $scope.displayedItems.isContextMenuDisplayed = false;
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

      $scope.closeContextMenu = function() {
        $scope.isContextMenuDisplayed = false;
      };

      function isTouchDevice() {
        return ('ontouchstart' in window) || navigator.maxTouchPoints > 0 || navigator.msMaxTouchPoints > 0 || window.innerWidth <= 991;
      }

      $scope.openContextMenu = function (e, item, lists, carts) {
        if (isTouchDevice()) {
          var modalInstance = $modal.open({
            templateUrl: 'views/contextmenumodal.html',
            controller: 'ContextMenuModalController',
            resolve: {
              lists: function () {
                return lists;
              },
              carts: function () {
                return carts;
              },
              item: function() {
                return item;
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