'use strict';

angular.module('bekApp')
.directive('contextMenu', ['ListService', function(ListService){
  return {
    restrict: 'A',
    scope: true,
    controller: ['$scope', '$state', 'ListService', 'OrderService', function($scope, $state, ListService, OrderService){

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

      $scope.addItemToOrder = function(orderId, item) {
        OrderService.addItemToOrder(orderId, item).then(function(data) {
          debugger;
        });
      };

      $scope.createOrderWithItem = function(item) {
        OrderService.createOrderWithItem(item).then(function(data) {
          debugger;
        });
      };

    }],
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);