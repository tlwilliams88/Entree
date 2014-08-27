'use strict';

angular.module('BEKPhoneGap')
.directive('favoriteIcons', ['ListService', function(ListService){
  return {
    restrict: 'A',
    templateUrl: 'views/directives/favoriteicons.html'
  };
}]);