'use strict';

angular.module('bekApp')
.directive('contextMenu', ['ListService', function(ListService){
  return {
    restrict: 'A',
    scope: true,
    controller: ['$scope', '$element', '$state', 'ListService', function($scope, $element, $state, ListService){

      $scope.addItemToList = function(listId, item) {
        $scope.loadingContextMenu = true;

        ListService.addItemToListAndFavorites(listId, item).then(function(data) {
          $scope.loadingContextMenu = false;
          $scope.isContextMenuDisplayed = false;
        });
      };

      $scope.createListWithItem = function(item) {
        ListService.createListWithItem(item).then(function(data) {
          $scope.loadingContextMenu = false;
          $state.go('menu.listitems', {listId: data.listitemid});
        });
      };

      $scope.removeItem = function(selectedList, item) {
        $scope.loadingContextMenu = true;

        ListService.deleteItem(selectedList.listid, item.listitemid).then(function(data) {
          $scope.loadingContextMenu = false;
          $scope.isContextMenuDisplayed = false;
        });
      };

    }],
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);