'use strict';

angular.module('bekApp')
.directive('orderCutoffDate', ['DateService' , function(DateService) {
  var directive = {
    require: 'ngModel',
    restrict: 'A',
    link: function(scope, elm, attrs, ctrl) {
      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);

      function checkValidity(viewValue) {
        if(scope.$modelValue || scope.$modelValue === undefined){

             var cutoffDate = DateService.momentObject(viewValue).format();             
             var now = DateService.momentObject().tz('America/Chicago').format();

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