'use strict';

angular.module('bekApp')
.directive('favoriteItemClickHandlers', [function(){
  return {
    restrict: 'A',
    replace : true,
    scope: true,
    // transclude: true,
    controller: ['$scope', '$element', 'ListService', function($scope, $element, ListService){

      $scope.addItemToFavorites = function(item) {
        $scope.$emit('processing-start');
        ListService.addItemToFavorites(item).then(function(data) {
          item.favorite = true;
          $scope.$emit('processing-end', { message: 'Added item to favorites.' });
        });
      };
      $scope.removeItemFromFavorites = function(item) {
        $scope.$emit('processing-start');
        ListService.removeItemFromFavorites(item).then(function(data) {
          item.favorite = false;
        });
        $scope.$emit('processing-end', { message: 'Removed item from favorites.' });
      };
    }],
    // template: '<span ng-transclude></span>'
  };
}]);