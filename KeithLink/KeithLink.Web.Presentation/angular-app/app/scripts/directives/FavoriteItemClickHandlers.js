'use strict';

angular.module('bekApp')
.directive('favoriteItemClickHandlers', [function(){
  return {
    restrict: 'A',
    replace : true,
    scope: true,
    controller: ['$scope', 'toaster', 'ListService', function($scope, toaster, ListService){

      $scope.addItemToFavorites = function(item) {
        // $scope.$emit('processing-start');
        var newItem = angular.copy(item);
        ListService.addItemToFavorites(newItem).then(function(data) {
          item.favorite = true;
          toaster.pop('success', null, 'Successfully added item to Favorites List.');
        }, function() {
          toaster.pop('error', null, 'Error adding item to Favorites List.');
        });
      };
      $scope.removeItemFromFavorites = function(item) {
        // $scope.$emit('processing-start');
        var deletedItem = angular.copy(item);
        ListService.removeItemFromFavorites(deletedItem.itemnumber).then(function(data) {
          item.favorite = false;
          toaster.pop('success', null, 'Successfully removed item from Favorites List.');
        }, function() {
          toaster.pop('error', null, 'Error removing item from Favorites List.');
        });
      };
    }]
  };
}]);