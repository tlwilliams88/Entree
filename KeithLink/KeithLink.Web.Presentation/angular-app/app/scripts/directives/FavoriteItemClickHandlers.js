'use strict';

angular.module('bekApp')
.directive('favoriteItemClickHandlers', [function(){
  return {
    restrict: 'A',
    replace : true,
    scope: true,
    controller: ['$scope', 'ListService', function($scope, ListService){

      $scope.addItemToFavorites = function(item) {
        // $scope.$emit('processing-start');
        var newItem = angular.copy(item);
        ListService.addItemToFavorites(newItem).then(function(data) {
          item.favorite = true;
          $scope.displayMessage('success', 'Successfully added item to Favorites List.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to Favorites List.');
        });
      };
      $scope.removeItemFromFavorites = function(item) {
        // $scope.$emit('processing-start');
        var deletedItem = angular.copy(item);
        ListService.removeItemFromFavorites(deletedItem.itemnumber).then(function(data) {
          item.favorite = false;
          $scope.displayMessage('success', 'Successfully removed item from Favorites List.');
        }, function() {
          $scope.displayMessage('error', 'Error removing item from Favorites List.');
        });
      };
    }]
  };
}]);