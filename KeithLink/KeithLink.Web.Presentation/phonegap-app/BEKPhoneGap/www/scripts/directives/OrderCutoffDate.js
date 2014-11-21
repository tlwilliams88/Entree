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

          var dateValue = viewValue;
          if (viewValue) { // IE and ipad don't support creating dates with YYYY-MM-DD 00:00 format
            dateValue = dateValue.split(' ')[0];
          }
          var cutoffDate = new Date(dateValue),
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