'use strict';

angular.module('BEKPhoneGap')
.directive('contextMenu', ['ListService', function(ListService){
  return {
    restrict: 'A',
    scope: true,
    controller: ['$scope', '$element', '$state', 'ListService', function($scope, $element, $state, ListService){

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

    }],
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);