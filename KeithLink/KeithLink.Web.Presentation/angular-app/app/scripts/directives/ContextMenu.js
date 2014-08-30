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
        $state.go('menu.lists.items', { listId: '2254cadf-c0f6-43ad-91c4-09a373ab48ed', renameList: true });
        
        // ListService.createListWithItem(item).then(function(data) {
        //   $scope.loadingContextMenu = false;
        //   $state.go('menu.lists.items', { listId: data[0].listid, renameList: true });
        // });
      };

      $scope.addItemToCart = function(cartId, item) {
        CartService.addItemToCart(cartId, item).then(function(data) {
          
        });
      };

      $scope.createCartWithItem = function(item) {
        $state.go('menu.cart.items', { cartId: '1971090b-bb40-4084-b17f-74988c8ce444', renameCart: true });
        // var items = [item];
        // CartService.createCart(items).then(function(data) {
        //   $state.go('menu.cart.items', { cartId: data.listitemid, renameCart: true });
        // });
      };

    }],
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);