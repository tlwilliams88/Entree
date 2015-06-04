'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:allowOnePositiveDecimal
 * @description
 * form validation where the input can have only one decimal place and must be positive
 *
 * used for list parlevel
 */
angular.module('bekApp')
.directive('allowOnePositiveDecimal',function(){
  var directive = {
    require: 'ngModel', 
    restrict: 'A', 
    link: function(scope, elm, attrs, ctrl) {
      function checkValidity(viewValue) {

        
        // add a leading zero if value starts with a decimal
        if (typeof viewValue === 'string' && viewValue.indexOf('.') === 0) {
          ctrl.$setViewValue('0' + viewValue);
          ctrl.$render();
        }

        if(viewValue && viewValue !== undefined){
          var reg = /[a-zA-Z]/;
          if(reg.test(viewValue)){
          viewValue = viewValue.replace(/[^0-9\.]+/g, '');
          ctrl.$setViewValue(viewValue);   
          ctrl.$render();
        }

        }
        if (attrs.id === 'inventoryRep' || attrs.id==="parlevel"  || attrs.id==="onHand") {
          //allows for 2 decimal places
         scope.checkRegex = (directive.REGEXP2.test(viewValue) || directive.REGEXP.test(viewValue));
        } else{
          scope.checkRegex = directive.REGEXP.test(viewValue);
        }

        if (!viewValue || scope.checkRegex ) {
          ctrl.$setValidity('allowOnePositiveDecimal', true);
          return parseFloat(viewValue);
        } else {
          ctrl.$setValidity('allowOnePositiveDecimal', false);
          return undefined;
        }
      }

      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);
    }, 
    REGEXP : /^([1-9]\d*|0)(\.\d)?$/,
    REGEXP2 : /^([1-9]\d*|0)(\.\d\d)?$/ 
  };

  return directive;
});