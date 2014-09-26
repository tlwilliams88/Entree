'use strict';

// only allows users to enter integer values into textbox
// also provides dropdown of values

angular.module('bekApp')
.directive('updateQuantityFromParlevel', [function() {
  var directive = {
    require: 'ngModel',
    restrict: 'A',
    scope: {
      parlevel: '=',
      quantity: '='
    },
    link: function(scope, elm, attrs, ctrl) {
      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);

      function checkValidity(viewValue) {
        if(scope.$modelValue || scope.$modelValue === undefined){
          var amountOnHand = viewValue;
          if (scope.parlevel > amountOnHand) {
            scope.quantity = Math.ceil(scope.parlevel - amountOnHand);
          } else {
            scope.quantity = null;
          }
          return viewValue;
        }
      }
    }
  };

  return directive;
}]);