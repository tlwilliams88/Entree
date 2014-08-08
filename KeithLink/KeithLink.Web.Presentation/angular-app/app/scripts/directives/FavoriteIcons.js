'use strict';

angular.module('bekApp')
.directive('favoriteIcons', ['ListService', function(ListService){
  return {
    restrict: 'A',
    templateUrl: 'views/directives/favoriteicons.html'
  };
}]);