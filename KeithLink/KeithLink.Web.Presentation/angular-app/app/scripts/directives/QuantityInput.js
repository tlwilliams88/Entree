'use strict';

angular.module('bekApp')
.directive('quantityInput', [function() {
  var directive = {
    replace: true,
    restrict: 'A',
    scope: {
      quantityValue: '=quantityInput',
      formControlClass: '=formControl'
    },
    link: function(scope, elm, attrs, ctrl) {
      scope.changeQuantity = function(qty) {
        scope.quantityValue = qty;
      };
    },
    templateUrl: 'views/directives/quantityinput.html'
  };

  return directive;
}]);