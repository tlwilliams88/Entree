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
          $state.go('menu.listitems', { listId: data[0].listid, renameList: true });
        });
      };

      $scope.addItemToCart = function(cartId, item) {
        CartService.addItemToCart(cartId, item).then(function(data) {
          
        });
      };

      $scope.createCartWithItem = function(item) {
        CartService.createCartWithItem(item).then(function(data) {
          
        });
      };

    }],
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);