'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:requiredArray
 * @description
 * Form validation that checks that an array has a length > 0
 */
angular.module('bekApp')
.directive('requiredArray', function () {
    
    return {
      require: '?ngModel',
      restrict: 'A',
      link: function(scope, elm, attr, ctrl) {
        if (!ctrl) {
          return;
        }

        ctrl.$validators.requiredArray = function(modelValue, viewValue) {
          return (viewValue > 0);
        };
      }
    };
  });