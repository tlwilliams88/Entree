'use strict';

angular.module('bekApp')
.directive('disableNgAnimate', function($animate){
  return {
    restrict: 'A',
    link: function(scope, element, attrs){
      $animate.enabled(false, element);
    }
  };
});