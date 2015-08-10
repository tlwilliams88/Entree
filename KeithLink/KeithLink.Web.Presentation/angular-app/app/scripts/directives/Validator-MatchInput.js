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
      if (!ctrl) { return; }

      ctrl.$validators.matchInput = function(modelValue, viewValue) {
        return ctrl.$isEmpty(modelValue) || scope.matchInput === modelValue;
      };

      scope.$watch('matchInput', function() {
        ctrl.$validate();
      });
    }
  };
});