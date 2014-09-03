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
    link: function(scope, elem, attrs, ctrl) {
      scope.$watch(function() {

        var isDuplicated = false;
        // loops through collection of items
        angular.forEach(scope.collection, function(item, index) {
          // checks if the given field of the item matches the input
          if (ctrl.$modelValue && item[attrs.checkDuplicateField].toUpperCase() === ctrl.$modelValue.toUpperCase()) {
            isDuplicated = true;
          }
        });

        return (ctrl.$pristine || angular.isUndefined(ctrl.$modelValue)) || !isDuplicated;
      }, function(currentValue) {
        ctrl.$setValidity('checkDuplicateField', currentValue);
      });
    }
  };
});