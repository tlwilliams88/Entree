'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:numericValidation
 * @description
 * Form validation to set number of allowable decimals and pull out invalid values
 * Used for list parlevel
 */
angular.module('bekApp')
.directive('numericValidation',function(){
  var directive = {
    require: 'ngModel', 
    restrict: 'A', 
    link: function(scope, elm, attrs, ctrl) {
      function checkValidity(viewValue) {
        if(!viewValue){
        ctrl.$setValidity('numericValidation', true);
        return 0;
        }
        // add a leading zero if value starts with a decimal
        if (typeof viewValue !== 'string') {
          viewValue = viewValue.toString();
        }
    
          if(viewValue.indexOf('.') === 0){
            viewValue = '0' + viewValue; 
            ctrl.$setViewValue(viewValue);   
            ctrl.$render();  
          }
          //Allow '0.' as an option, but keep it invalid
          if(viewValue ==='0.'){
            ctrl.$render();
           ctrl.$setValidity('numericValidation', false);
          return parseFloat(viewValue);
          }

        var truncatedVal ='';
        var allowTwoDecimals = (attrs.id === 'inventoryRep' || attrs.id==="parlevel"  || attrs.id==="onHand" || attrs.id.indexOf('defaultElement') != -1)
        if (allowTwoDecimals) {
          //allows for 2 decimal places
          truncatedVal = truncateViewValue(2,viewValue);
          scope.checkRegex = (directive.REGEXP2.test(truncatedVal) || directive.REGEXP.test(truncatedVal));
        } else{
          truncatedVal =  truncateViewValue(1,viewValue);
          scope.checkRegex = directive.REGEXP.test(truncatedVal);
        }
        //set validity
        if (!truncatedVal || scope.checkRegex ) {
          ctrl.$setValidity('numericValidation', true);
          return parseFloat(truncatedVal);
        } else {
          ctrl.$setValidity('numericValidation', false);
          return undefined;
        }
      }

      function truncateViewValue(limit,viewVal){
        //filter out special/alpha characters and remove leading 0s
           if(typeof viewVal === 'string'){
            if(viewVal.length>1 && viewVal.indexOf(0) === 0 && viewVal[1] !== '.'){
              viewVal = viewVal.slice(1,viewVal.length);
                ctrl.$setViewValue(viewVal);   
                ctrl.$render(); 
            }

            for (var i = 0, length = viewVal.length; i < length; i++) {
              if(['1','2','3','4','5','6','7','8','9','0','.'].indexOf(viewVal[i]) === -1){
                viewVal = viewVal.replace(viewVal[i], '');         
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
        if(!viewVal.length || viewVal === '0.0' || viewVal === '0.00'){
          viewVal = '0';
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
