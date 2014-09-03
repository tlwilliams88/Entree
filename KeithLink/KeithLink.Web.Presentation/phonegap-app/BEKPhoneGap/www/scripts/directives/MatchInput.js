'use strict';

angular.module('bekApp')
.directive('matchInput', function () {
        return {
            require: 'ngModel',
            restrict: 'A',
            scope: {
                matchInput: '='
            },
            link: function(scope, elem, attrs, ctrl) {
                scope.$watch(function() {
                    return (ctrl.$pristine || angular.isUndefined(ctrl.$modelValue)) || scope.matchInput === ctrl.$modelValue;
                }, function(currentValue) {
                    ctrl.$setValidity('matchInput', currentValue);
                });
            }
        };
    });