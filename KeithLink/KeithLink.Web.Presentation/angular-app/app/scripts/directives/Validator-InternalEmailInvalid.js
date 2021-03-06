'use strict';

/**
 * internalEmailInvalid directive
 * Checks for internal BEK emails and does not allow them
 */
angular.module('bekApp')
.directive('internalEmailInvalid', [function() {
  var directive = {
    require: 'ngModel',
    restrict: 'A',
    link: function(scope, elm, attr, ctrl) {
      if (!ctrl) { return; }
      
      ctrl.$validators.internalEmailInvalid = function(modelValue, viewValue) {
        return ctrl.$isEmpty(modelValue) || modelValue.indexOf('@benekeith.com') === -1;
      };

    }
  };

  return directive;
}]);