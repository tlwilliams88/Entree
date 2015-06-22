'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:allowOnePositiveDecimal
 * @description
 * Form validation where the input can have only one decimal place and must be positive. 
 * Used for list parlevel
 */
angular.module('bekApp')
.directive('allowOnePositiveDecimal',function(){
  var directive = {
    require: 'ngModel', 
    restrict: 'A', 
    link: function(scope, elm, attrs, ctrl) {
      function checkValidity(viewValue) {
        // add a leading zero if value starts with a decimal
        if (typeof viewValue === 'string') {
          if(viewValue.indexOf('.') === 0){
            viewValue = '0' + viewValue; 
            ctrl.$setViewValue(viewValue);   
            ctrl.$render();  
          }
          //Allow '0.' as an option, but keep it invalid
          if(viewValue ==='0.'){
            ctrl.$render();
           ctrl.$setValidity('allowOnePositiveDecimal', false);
          return parseFloat(viewValue);
          }    
        }
    
        var truncatedVal ='';
        if (attrs.id === 'inventoryRep' || attrs.id==="parlevel"  || attrs.id==="onHand") {
          //allows for 2 decimal places
          truncatedVal = truncateDecimals(2,viewValue);
          scope.checkRegex = (directive.REGEXP2.test(truncatedVal) || directive.REGEXP.test(truncatedVal));
        } else{
          truncatedVal =  truncateDecimals(1,viewValue);
          scope.checkRegex = directive.REGEXP.test(truncatedVal);
        }
        //set validity
        if (!truncatedVal || scope.checkRegex ) {
          ctrl.$setValidity('allowOnePositiveDecimal', true);
          return parseFloat(truncatedVal);
        } else {
          ctrl.$setValidity('allowOnePositiveDecimal', false);
          return undefined;
        }
      }

      function truncateDecimals(limit,viewVal){
        //filter out special/alpha characters
           if(typeof viewVal === 'string'){
            var parsed = '';
          for (var i = 0, length = viewVal.length; i < length; i++) {
            if(['1','2','3','4','5','6','7','8','9','0','.'].indexOf(viewVal[i]) === -1){
              viewVal = viewVal.replace(viewVal[i], '');
              ctrl.$setValidity('allowOnePositiveDecimal', true);          
              ctrl.$setViewValue(viewVal);   
              ctrl.$render();    
            }
          }            
        } 
        //truncate decimals to correct length
         if(viewVal && typeof viewVal === 'string' && viewVal.indexOf('.') !== -1 && viewVal.slice(viewVal.indexOf('.'),(viewVal.length-1)).length > limit ){             
            viewVal = viewVal.slice(0,(viewVal.indexOf('.') + limit + 1)); 
             ctrl.$setViewValue(viewVal);   
             ctrl.$render();         
          }   
        return viewVal;
      }

      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);
    }, 
    REGEXP : /^([1-9]\d*|0)(\.\d)?$/,
    REGEXP2 : /^([1-9]\d*|0)(\.\d\d)?$/ 
  };

  return directive;
});