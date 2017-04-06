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
          var username_check = new RegExp(attrs.username, 'i'),
              first_name_check = new RegExp(attrs.firstName, 'i'),
              last_name_check = new RegExp(attrs.lastName, 'i'),
              password_length_check = viewValue ? viewValue.length > 7 : false;

          return ctrl.$isEmpty(modelValue) || ( UPPERCASE_REGEX.test(viewValue) && 
            LOWERCASE_REGEX.test(viewValue) && NUMBER_REGEX.test(viewValue) && 
            password_length_check && !username_check.test(viewValue) && 
            !first_name_check.test(viewValue) && !last_name_check.test(viewValue) );
        };
      }
    };
  });
