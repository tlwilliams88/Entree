'use strict';

angular.module('bekApp')
.directive('orderCutoffDate', [function() {
  var directive = {
    require: 'ngModel',
    restrict: 'A',
    link: function(scope, elm, attrs, ctrl) {
      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);

      function checkValidity(viewValue) {
        if(scope.$modelValue || scope.$modelValue === undefined){

          var cutoffDate = viewValue,
            now = new Date();

          if (now < cutoffDate) {
            
            // valid, cutoff datetime has not past
            ctrl.$setValidity('orderCutoffDate', true);
            return viewValue;
          } else {
            
            // it is invalid, return undefined (no model update)
            ctrl.$setValidity('orderCutoffDate', false);
            return undefined;
          }
        }
      }
    }
  };

  return directive;
}]);