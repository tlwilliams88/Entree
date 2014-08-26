'use strict';

angular.module('bekApp')
.directive('checkDuplicateField', function () {
    return {
    require: 'ngModel',
    restrict: 'A',
    scope: {
      collection: '='
    },
    link: function(scope, elem, attrs, ctrl) {
      scope.$watch(function() {

        var isDuplicated = false;
        angular.forEach(scope.collection, function(item, index) {
          if (ctrl.$modelValue && item[attrs.checkDuplicateField].toUpperCase() === ctrl.$modelValue.toUpperCase()) {
            isDuplicated = true;
          }
        });

        return (ctrl.$pristine || angular.isUndefined(ctrl.$modelValue)) || !isDuplicated;
      }, function(currentValue) {
        ctrl.$setValidity('checkDuplicateField', currentValue);
      });
    }
  };
});