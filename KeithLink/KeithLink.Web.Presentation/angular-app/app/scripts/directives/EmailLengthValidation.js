 'use strict';

// /**
//  * @ngdoc function
//  * @name bekApp.directive:emailLengthValidation
//  * @description
//  * form validation where the local part of a new user's e-mail address can not exceed 20 characters
//  *
//  * used for list parlevel
//  */

angular.module('bekApp')
.directive('emailLengthValidation',function(){
  var directive = {
    require: 'ngModel', 
    restrict: 'A', 
    link: function(scope, elm, attrs, ctrl) {
      function checkValidity(viewValue) {
        
        if ((viewValue.indexOf('@') < 21)) {
          ctrl.$setValidity('emailLengthValidation', true);
          return viewValue;         
        } else {
          ctrl.$setValidity('emailLengthValidation', false);
          return undefined;         
        }
      }

      ctrl.$parsers.unshift(checkValidity);
      ctrl.$formatters.unshift(checkValidity);
    }, 
  };

  return directive;
});

