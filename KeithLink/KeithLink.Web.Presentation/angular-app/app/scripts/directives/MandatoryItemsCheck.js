'use strict';

angular.module('bekApp')
.directive('mandatoryItemsCheck', [function() {
  var directive = {
    require: 'ngModel',
    restrict: 'A',
    link: function(scope, elm, attrs, ctrl) {
      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);

      function checkValidity(viewValue) {
        if(scope.$modelValue || scope.$modelValue === undefined){

          if (viewValue === 0) {
            
            // valid, no missing mandatory items
            ctrl.$setValidity('mandatoryItemsCheck', true);
            return viewValue;
          } else {
            
            // it is invalid, return undefined (no model update)
            ctrl.$setValidity('mandatoryItemsCheck', false);
            return undefined;
          }
        }
      }
    }
  };

  return directive;
}]);