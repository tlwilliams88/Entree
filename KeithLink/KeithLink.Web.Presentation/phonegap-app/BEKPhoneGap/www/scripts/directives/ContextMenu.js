'use strict';

angular.module('bekApp')
.directive('contextMenu', [ function(){
  return {
    restrict: 'A',
    scope: true,
    controller: ['$scope', '$state', '$q', 'toaster', 'ListService', 'CartService', 
    function($scope, $state, $q, toaster, ListService, CartService){

      $scope.addItemToList = function(listId, item) {
        var newItem = angular.copy(item);
        $scope.loadingContextMenu = true;

        $q.all([
          ListService.addItem(listId, item),
          ListService.addItemToFavorites(item)
        ]).then(function(data) {
          item.favorite = true;
          $scope.loadingContextMenu = false;
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
          $scope.loadingContextMenu = false;
          $state.go('menu.lists.items', { listId: data[0].listid, renameList: true });
          $scope.displayMessage('success', 'Successfully created new list.');
        }, function() {
          $scope.displayMessage('error', 'Error creating new list.');
        });
      };

      $scope.addItemToCart = function(cartId, item) {
        $scope.loadingContextMenu = true;
        CartService.addItemToCart(cartId, item).then(function(data) {
          $scope.loadingContextMenu = false;
          $scope.displayedItems.isContextMenuDisplayed = false;
          $scope.displayMessage('success', 'Successfully added item to cart.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to cart.');
        });
      };

      $scope.createCartWithItem = function(item) {
        var items = [item];
        $scope.loadingContextMenu = true;
        CartService.createCart(items).then(function(data) {
          $scope.loadingContextMenu = false;
          $state.go('menu.cart.items', { cartId: data.id, renameCart: true });
          $scope.displayMessage('success', 'Successfully created new cart.');
        }, function() {
          $scope.displayMessage('error', 'Error creating new cart.');
        });
      };
    }],
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);