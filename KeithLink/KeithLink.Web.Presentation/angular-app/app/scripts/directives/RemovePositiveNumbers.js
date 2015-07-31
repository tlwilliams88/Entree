
'use strict';



angular.module('bekApp')
.directive('removePositiveNumbers',function(){
  var directive = {
    require: 'ngModel', 
    restrict: 'A', 
    link: function(scope, elm, attrs, ctrl) {
      function checkValidity(viewValue) {
      //If invoice is credit, re-initialize the field when positive numbers are entered      
       var invAmt = scope.$eval(attrs.invoiceAmount);         
        if( viewValue !== undefined && viewValue !== 0 && invAmt < 0 && viewValue.slice(0,2).indexOf('-')===-1 && attrs.status !== 'Payment Pending'){
          ctrl.$setViewValue('');
          ctrl.$render();                     
          return '';        
        }
        else {       
          return viewValue;         
        }     
      }
       ctrl.$parsers.unshift(checkValidity);
       ctrl.$formatters.unshift(checkValidity);
    }, 
  };

  return directive;
});
