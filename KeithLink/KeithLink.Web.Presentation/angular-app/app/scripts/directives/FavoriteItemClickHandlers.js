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
        var newItem = angular.copy(item);
        ListService.addItemToFavorites(newItem).then(function(data) {
          item.favorite = true;
        });
      };
      $scope.removeItemFromFavorites = function(item) {
        $scope.$emit('processing-start');
        var deletedItem = angular.copy(item);
        ListService.removeItemFromFavorites(deletedItem.itemnumber).then(function(data) {
          item.favorite = false;
        });
      };
    }],
    // template: '<span ng-transclude></span>'
  };
}]);