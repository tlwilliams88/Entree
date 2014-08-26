'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:favoriteIcons
 * @description
 * displays confirmation message and executes the given function if message is approved
 *
 * used on search results and lists page
 */
angular.module('bekApp')
.directive('favoriteIcons', ['ListService', function(ListService){
  return {
    restrict: 'A',
    templateUrl: 'views/directives/favoriteicons.html'
  };
}]);