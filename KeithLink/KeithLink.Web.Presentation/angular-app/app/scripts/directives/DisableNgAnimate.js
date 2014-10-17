'use strict';

angular.module('bekApp')
.directive('disableNgAnimate', function($animate){
    return {
        restrict: 'A',
        link: function($scope, $element, $attrs){
            $attrs.$observe('disableNgAnimate', function(value){
                $animate.enabled(!value, $element);
            });
        }
    }
});