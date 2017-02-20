'use strict';

/**
 * input validation directive
 * Checks for special characters and disallows submitting if any are found
 */

angular.module('bekApp')
.directive('validateInput', [function() {
  var directive = {
    require: 'ngModel',
    restrict: 'A',
    link: function(scope, elm, attr, ctrl) {
      if (!ctrl) { return; }
      
      ctrl.$validators.searchInputInvalid = function(viewValue) {
        var inputIsValid = directive.REGEXP.test(viewValue);
        ctrl.$setValidity('validateInput', inputIsValid);
        return inputIsValid;
      };

    },
    REGEXP : /^[a-z A-Z 0-9]*$/
  };

  return directive;
}]);