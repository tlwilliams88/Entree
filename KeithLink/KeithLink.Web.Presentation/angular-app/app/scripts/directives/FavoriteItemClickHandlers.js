'use strict';

angular.module('bekApp')
.directive('favoriteItemClickHandlers', [function(){
  return {
    restrict: 'A',
    replace : true,
    scope: true,
    controller: ['$scope', 'ListService', function($scope, ListService){

      var processingAddItem = false;
      $scope.addItemToFavorites = function(item) {
        var newItem = angular.copy(item);

        if (!processingAddItem) {
          processingAddItem = true;
          
          ListService.addItemToFavorites(newItem).then(function(data) {
            item.favorite = true;
            $scope.displayMessage('success', 'Successfully added item to Favorites List.');
          }, function() {
            $scope.displayMessage('error', 'Error adding item to Favorites List.');
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
            item.favorite = false;
            $scope.displayMessage('success', 'Successfully removed item from Favorites List.');
          }, function() {
            $scope.displayMessage('error', 'Error removing item from Favorites List.');
          }).finally(function() {
            processingAddItem = false;
          });
        }
      };
    }]
  };
}]);