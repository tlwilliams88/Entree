'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:allowOnePositiveDecimal
 * @description
 * form validation where the input value cannot match the given field in the given array of objects
 * is not case sensitive
 *
 * Inputs:
 * checkDuplicateField: object property name to check against
 * collection: array of JS objects to check against for duplicates
 *
 * used for list and order renaming
 */
angular.module('bekApp')
.directive('checkDuplicateField', function () {
    return {
    require: 'ngModel',
    restrict: 'A',
    scope: {
      collection: '='
    },
    link: function(scope, elm, attrs, ctrl) {
      function checkValidity(viewValue) {

        var isDuplicate = false;
        if (viewValue !== ctrl.$modelValue) { // check only if the user has tried to change the name
          angular.forEach(scope.collection, function(item, index) {
            if (item[attrs.checkDuplicateField].toUpperCase() === viewValue.toUpperCase()) {
              isDuplicate = true;
            }
          });
        }

        if (!isDuplicate) {
          ctrl.$setValidity('checkDuplicateField', true);
          return viewValue;
        } else {
          ctrl.$setValidity('checkDuplicateField', false);
          return ctrl.$modelValue;
        }
      }

      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);
    }
  };
});