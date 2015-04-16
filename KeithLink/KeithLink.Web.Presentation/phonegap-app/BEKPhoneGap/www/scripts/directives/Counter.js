'use strict';

angular.module('bekApp')
  .directive('counter', ['$filter', '$window', function($filter, $window) {
    return {
      restrict: 'A',
      require: ['^form','ngModel'],
      scope: { 
        value: '=ngModel'
      },
      templateUrl: 'views/directives/counter.html',
      link: function( scope, element, attributes, ctrls ) {

        var formCtrl = ctrls[0]; // form controller
        var ctrl = ctrls[1]; // ngmodelcontroller

        // set size
        element.addClass('counter-container');
        if (attributes.size === 'large') { // item details page
          element.addClass('counter-container-lg');
        }

        // Make sure the value attribute is not missing.
        if ( angular.isUndefined(scope.value) ) {
          // throw "Missing the value attribute on the counter directive.";
        }
        
        // if (attributes.allowdropdown === 'true') {
        //   scope.allowDropdown = true;
        // }

        // if (!scope.value) {
        //   scope.value = 0;
        // }
        
		    var min = angular.isUndefined(attributes.min) ? null : parseInt(attributes.min);
        var max = angular.isUndefined(attributes.max) ? null : parseInt(attributes.max);
        var step = angular.isUndefined(attributes.step) ? 1 : parseInt(attributes.step);
		    var length = angular.isUndefined(attributes.length) ? null : parseInt(attributes.length);
		  
        // If the 'editable' attribute is set, we will make the field editable.
        scope.readonly = angular.isUndefined(attributes.editable) ? true : false;
        
        /**
         * Sets the value as an integer.
         */
        var setValue = function( val ) {
          scope.value = parseInt( val );
          // ctrl.$setViewValue(val);
          formCtrl.$setDirty(true);

          disableButtons(val);
          // scope.counter.isopen = false;
        };

        scope.newValue = setValue;

        var disableButtons = function(val) {
          if (!val) {
            val = 0;
          }
          if (min !== null) {
            scope.disabledMinus = (val <= min);
          }
          if (max !== null) {
            scope.disabledPlus = (val >= max);
          }
        };
        
        // Set the value initially, as an integer.
        // setValue( scope.value );
        disableButtons(scope.value);
        
        /**
         * Decrement the value and make sure we stay within the limits, if defined.
         */
        scope.minus = function() {
          if ( min && (scope.value <= min || scope.value - step <= min) || min === 0 && scope.value < 1 ) {
            setValue( min );
            return false;
          }
          var val = scope.value ? scope.value : 0; // default to value of 0 if undefined
          setValue( val - step );
        };
        
        /**
         * Increment the value and make sure we stay within the limits, if defined.
         */
        scope.plus = function() {
          if ( max && (scope.value >= max || scope.value + step >= max) ) {
            setValue( max );
            return false;
          }
          var val = scope.value ? parseInt(scope.value, 10) : 0; // default to value of 0 if undefined
          setValue( val + step );
        };
        
        /**
         * This is only triggered when the field is manually edited by the user.
         * Where we can perform some validation and make sure that they enter the
         * correct values from within the restrictions.
         */
        scope.changed = function() {
          var val = scope.value ? scope.value : 0; // default to value of 0 if undefined
          
          var curLength = parseInt(val.toString().length);

          // Check if what's typed is numeric or if it has any letters.
          if ( /\b[0-9]+\b/.test(val) ) {
            if(length != null){ // jshint ignore:line
              if(curLength <= length){
                if (val === 0) {
                  val = null;
                }
                setValue(val);
              } else{
                setValue( parseInt( val.toString().substring(0,length), 10 ) );
              }
            } else{
              setValue(val);
            }
          }
          else {
            setValue( parseInt( val.toString().substring(0,curLength - 1), 10 ) );
          }

          // If a minimum is set, let's make sure we're within the limit.
          if ( min && (val <= min || val - step <= min) ) {
            setValue( min );
            return false;
          }

          // If a maximum is set, let's make sure we're within the limit.
          if ( max && (val >= max || val + step >= max) ) {
            setValue( max );
            return false;
          }
		  
          
          // Re-set the value as an integer.
          setValue( val );
        };

        scope.confirmQuantity = function(qty) {
          var pattern = /^([0-9])\1+$/; // repeating digits pattern

          if (qty > 50 || pattern.test(qty)) {
            var isConfirmed = $window.confirm('Do you want to continue with entered quatity of ' + qty + '?');
            if (!isConfirmed) {
              // clear input
              scope.value = null;
            }
          } 
        };
    }
  };
}]);