'use strict';

angular.module('bekApp')
.directive('validPaymentAmount', [function() {
  var directive = {
    require: 'ngModel',
    restrict: 'A',
    scope: {
      amountDue: '=validPaymentAmount'
    },
    link: function(scope, elm, attrs, ctrl) {
      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);

      function checkValidity(viewValue) {
        if(scope.$modelValue || scope.$modelValue === undefined){
          if ( parseFloat(viewValue) <= scope.amountDue ) {
           // it is valid
            ctrl.$setValidity('validPaymentAmount', true);
            return viewValue;
          } else {
            // it is invalid, return undefined (no model update)
            ctrl.$setValidity('validPaymentAmount', false);
            return undefined;
          }
        }
      }
    }
  };

  return directive;
}]);