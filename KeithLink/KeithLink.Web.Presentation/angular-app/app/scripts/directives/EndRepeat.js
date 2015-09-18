'use strict';

angular.module('bekApp')
.directive('endRepeat', function () {
    return {
        restrict: 'A',
        link: function (scope, elem, attr, ctrl) {
            if (scope.$last) {
                scope.$emit('repeat-ended');
            }
        }
    }
});