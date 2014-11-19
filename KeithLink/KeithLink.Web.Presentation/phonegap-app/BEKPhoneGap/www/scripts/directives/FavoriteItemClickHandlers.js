'use strict';

angular.module('bekApp')
.directive('favoriteItemClickHandlers', [function(){
  return {
    restrict: 'A',
    // replace : true,
    // scope: true,
    controller: ['$scope', 'ListService', function($scope, ListService){

      var processingAddItem = false;
      $scope.addItemToFavorites = function(item) {
        var newItem = angular.copy(item);

        if (!processingAddItem) {
          processingAddItem = true;
          var favoritesList = ListService.getFavoritesList();
          ListService.addItem(favoritesList.listid, item).then(function() {
            $scope.$broadcast('closeContextMenu');
            item.favorite = true;
          }).finally(function() {
            processingAddItem = false;
          });
        }
      };

      var processingRemoveItem = false;
      $scope.removeItemFromFavorites = function(item) {
        var deletedItem = angular.copy(item);

        if (!processingRemoveItem) {
          processingRemoveItem = true;

          ListService.removeItemFromFavorites(deletedItem.itemnumber).then(function(data) {
            $scope.$broadcast('closeContextMenu');
            item.favorite = false;
          }).finally(function() {
            processingRemoveItem = false;
          });
        }
      };
    }]
  };
}]);