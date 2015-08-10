'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:passwordRequirements
 * @description
 * Passwords must have 1 uppercase, 1 lowercase, and 1 number
 */
angular.module('bekApp')
.directive('passwordRequirements', function () {
    
    var UPPERCASE_REGEX = /[A-Z]/,
      LOWERCASE_REGEX = /[a-z]/,
      NUMBER_REGEX = /[0-9]/;

    return {
      require: 'ngModel',
      restrict: 'A',
      link: function(scope, elm, attrs, ctrl) {
        if (!ctrl) { return; }

        ctrl.$validators.passwordRequirements = function(modelValue, viewValue) {
          return ctrl.$isEmpty(modelValue) || ( UPPERCASE_REGEX.test(viewValue) && LOWERCASE_REGEX.test(viewValue) && NUMBER_REGEX.test(viewValue) && viewValue.length >7 );
        };
      }
    };
  });