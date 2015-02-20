'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:allowOnePositiveDecimal
 * @description
 * form validation where the input can have only one decimal place and must be positive
 *
 * used for list parlevel
 */
angular.module('bekApp')
.directive('allowOnePositiveDecimal',function(){
  var directive = {
    require: 'ngModel', 
    restrict: 'A', 
    link: function(scope, elm, attrs, ctrl) {
      function checkValidity(viewValue) {
        // add a leading zero if value starts with a decimal
        if (typeof viewValue === 'string' && viewValue.indexOf('.') === 0) {
          ctrl.$setViewValue('0' + viewValue);
          ctrl.$render();
        }


        if (!viewValue || directive.REGEXP.test(viewValue)) {
          ctrl.$setValidity('allowOnePositiveDecimal', true);
          return parseFloat(viewValue);
        } else {
          ctrl.$setValidity('allowOnePositiveDecimal', false);
          return undefined;
        }
      }

      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);
    }, 
    REGEXP : /^([1-9]\d*|0)(\.\d)?$/
  };

  return directive;
});