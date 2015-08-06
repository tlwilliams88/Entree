'use strict';

angular.module('bekApp')
.directive('postPageLoad', [function(){
  return {
    restrict: 'A',
    // replace : true,
    // scope: true,
    controller: ['$scope', 'ListService', '$timeout', function($scope, ListService, $timeout){

    if($scope.$last){
        ListService.setLoadPage(true);
        }     

    }]
  };
}]);