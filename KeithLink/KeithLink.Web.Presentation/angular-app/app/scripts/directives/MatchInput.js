'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:sortIcons
 * @description
 * form validation that requires the input to match the given matchInput attr
 */
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