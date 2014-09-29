'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:validatePassword
 * @description
 */
angular.module('bekApp')
.directive('validatePassword', function () {
    
    var UPPERCASE_REGEX = /[A-Z]/,
      LOWERCASE_REGEX = /[a-z]/,
      NUMBER_REGEX = /[0-9]/;

    return {
      require: 'ngModel',
      restrict: 'A',
      link: function(scope, elm, attrs, ctrl) {
        ctrl.$parsers.unshift(checkValidity);
        
        function checkValidity(viewValue) {

          if(scope.$modalValue || scope.$modalValue === undefined) {
            if ( UPPERCASE_REGEX.test(viewValue) && LOWERCASE_REGEX.test(viewValue) && NUMBER_REGEX.test(viewValue) ) {
             // it is valid
              ctrl.$setValidity('validatePassword', true);
              return viewValue;
            } else {
              // it is invalid, return undefined (no model update)
              ctrl.$setValidity('validatePassword', false);
              return undefined;
            }
          }
        }
      }
    };
  });