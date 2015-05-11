'use strict';

// users are only allowed to enter integer values into textbox

angular.module('bekApp')
.directive('integer', [function() {
  var directive = {
    require: 'ngModel',
    restrict: 'A',
    link: function(scope, elm, attrs, ctrl) {
      ctrl.$parsers.unshift(checkValidity);
      // ctrl.$formatters.unshift(checkValidity);

      function checkValidity(viewValue) {
        if(scope.$modelValue || scope.$modelValue === undefined){
          var regex = directive.INTEGER_REGEXP;
          if (attrs.allowLeadingZeroes === 'true') {
            regex = directive.INTEGER_LEADING_ZEROS;
          }
          if (regex.test(viewValue)) {
            // it is valid
            ctrl.$setValidity('integer', true);
            return viewValue;
          } else {
            var digits;
            if (viewValue) {
              digits = viewValue.split('').filter(function (s) { return (!isNaN(s) && s !== ' '); }).join('');
              ctrl.$setViewValue(parseInt(digits));
              ctrl.$render();
            }
            
            // it is invalid, return undefined (no model update)
            // ctrl.$setValidity('integer', false);
            return digits;
          }
        }
      }
    },
    INTEGER_REGEXP : /^\+?(0|[1-9]\d*)$/,
    INTEGER_LEADING_ZEROS : /^\+?([0-9]\d*)$/
  };

  return directive;
}]);