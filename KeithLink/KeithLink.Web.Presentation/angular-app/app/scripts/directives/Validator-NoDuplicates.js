'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:noDuplicates
 * @description
 * form validation where the input value cannot match the given field in the given array of objects
 * is not case sensitive
 *
 * Inputs:
 * noDuplicates: object property name to check against
 * collection: array of JS objects to check against for duplicates
 *
 * used for list and order renaming
 */
angular.module('bekApp')
.directive('noDuplicates', function () {
    return {
    require: 'ngModel',
    restrict: 'A',
    scope: {
      collection: '='
    },
    link: function(scope, elm, attrs, ctrl) {
      if (!ctrl) { return; }

      ctrl.$validators.noDuplicates = function(modelValue, viewValue) {
        var isDuplicate = false;
        if (viewValue !== ctrl.$modelValue) { // check only if the user has tried to change the name
          angular.forEach(scope.collection, function(item, index) {
            if (item[attrs.noDuplicates] != null && item[attrs.noDuplicates].toUpperCase() === viewValue.toUpperCase()) {
              isDuplicate = true;
            }
          });
        }

        return !isDuplicate;
      };
    }
  };
});
