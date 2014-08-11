'use strict';

angular.module('bekApp')
.directive('allowOnePositiveDecimal',function(){
  var directive = {
    require: 'ngModel'
    , restrict: "A"
    , scope:{
      number:'='
    }
    , link: function(scope, elm, attrs, ctrl) {
      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);

      function checkValidity(viewValue) {
        if(scope.number || scope.number === undefined){
          if (directive.INTEGER_REGEXP.test(viewValue)) {
            // it is valid
            ctrl.$setValidity('allowOnePositiveDecimal', true);
            return viewValue;
          } else {
            // it is invalid, return undefined (no model update)
            ctrl.$setValidity('allowOnePositiveDecimal', false);
            return undefined;
          }
        }
      };
    }
    , INTEGER_REGEXP : /^\+?(0|[1-9]\d*)$/
  };

  return directive;
});