'use strict';

angular.module('bekApp')
.directive('validateEmail', [function() {
  var directive = {
    require: 'ngModel',
    restrict: 'A',
    link: function(scope, elm, attr, ctrl) {
      if (!ctrl) { return; }
      attr.validateEmail = true; // force truthy in case we are on non input element

      ctrl.$validators.validateEmail = function(modelValue, viewValue) {
        // returning true means valid
        // validation failes when: 1) validateEmail = 'true', 2) email regex fails, 3) model is not empty
        return !( 
          attr.validateEmail === 'true' && 
          !directive.EMAIL_REGEXP.test(modelValue) && 
          !ctrl.$isEmpty(modelValue) 
        );
      };

      attr.$observe('validateEmail', function() {
        ctrl.$validate();
      });
    },
    EMAIL_REGEXP : /^[a-z]+[a-z0-9-._]+@[a-z]+\.[a-z.]{2,5}$/
  };

  return directive;
}]);