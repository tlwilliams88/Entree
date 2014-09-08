'use strict';

angular.module('bekApp')
.directive('contextMenu', ['ListService', function(ListService){
  return {
    restrict: 'A',
    scope: true,
    controller: ['$scope', '$state', 'ListService', 'CartService', function($scope, $state, ListService, CartService){

      $scope.addItemToList = function(listId, item) {
        var newItem = angular.copy(item);
        $scope.loadingContextMenu = true;

        ListService.addItemToListAndFavorites(listId, newItem).then(function(data) {
          item.favorite = true;
          $scope.loadingContextMenu = false;
          $scope.displayedItems.isContextMenuDisplayed = false;
        });
      };

      $scope.createListWithItem = function(item) {
        ListService.createListWithItem(item).then(function(data) {
          $scope.loadingContextMenu = false;
          $state.go('menu.lists.items', { listId: data[0].listid, renameList: true });
        });
      };

      $scope.addItemToCart = function(cartId, item) {
        $scope.loadingContextMenu = true;
        CartService.addItemToCart(cartId, item).then(function(data) {
          $scope.loadingContextMenu = false;
          $scope.displayedItems.isContextMenuDisplayed = false;
        });
      };

      $scope.createCartWithItem = function(item) {
        var items = [item];
        $scope.loadingContextMenu = true;
        CartService.createCart(items).then(function(data) {
          $scope.loadingContextMenu = false;
          $state.go('menu.cartitems', { cartId: data.listitemid, renameCart: true });
        });
      };

    }],
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);