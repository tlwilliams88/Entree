angular.module('bekApp')
  .directive('counter', function() {
    return {
      restrict: 'A',
      require: 'ngModel',
      scope: { value: '=ngModel' },
      templateUrl: 'views/directives/counter.html',
      link: function( scope, element, attributes, ctrl ) {
        // Make sure the value attribute is not missing.
        if ( angular.isUndefined(scope.value) ) {
          // throw "Missing the value attribute on the counter directive.";
        }

        if (!scope.value) {
          scope.value = 0;
        }
        scope.disabledMinus = false;
        scope.disabledPlus = false;
        
        var min = angular.isUndefined(attributes.min) ? null : parseInt(attributes.min);
        var max = angular.isUndefined(attributes.max) ? null : parseInt(attributes.max);
        var step = angular.isUndefined(attributes.step) ? 1 : parseInt(attributes.step);
        
        // element.addClass('counter-container');
        
        // If the 'editable' attribute is set, we will make the field editable.
        scope.readonly = angular.isUndefined(attributes.editable) ? true : false;
        
        /**
         * Sets the value as an integer.
         */
        var setValue = function( val ) {
          scope.value = parseInt( val );
          ctrl.$setViewValue(val);

          if (min !== null) {
            scope.disabledMinus = (val <= min);
          }
          if (max !== null) {
            scope.disabledPlus = (val >= max);
          }
        }
        
        // Set the value initially, as an integer.
        // setValue( scope.value );
        
        /**
         * Decrement the value and make sure we stay within the limits, if defined.
         */
        scope.minus = function() {
          if ( min && (scope.value <= min || scope.value - step <= min) || min === 0 && scope.value < 1 ) {
            setValue( min );
            return false;
          }
          setValue( scope.value - step );
        };
        
        /**
         * Increment the value and make sure we stay within the limits, if defined.
         */
        scope.plus = function() {
          if ( max && (scope.value >= max || scope.value + step >= max) ) {
            setValue( max );
            return false;
          }
          setValue( scope.value + step );
        };
        
        /**
         * This is only triggered when the field is manually edited by the user.
         * Where we can perform some validation and make sure that they enter the
         * correct values from within the restrictions.
         */
        scope.changed = function() {
            // If the user decides to delete the number, we will set it to 0.
          if ( !scope.value ) setValue( 0 );
          
          // Check if what's typed is numeric or if it has any letters.
          if ( /[0-9]/.test(scope.value) ) {
            setValue( scope.value );
          }
          else {
            setValue( scope.min );
          }
          
          // If a minimum is set, let's make sure we're within the limit.
          if ( min && (scope.value <= min || scope.value - step <= min) ) {
            setValue( min );
            return false;
          }
          
          // If a maximum is set, let's make sure we're within the limit.
          if ( max && (scope.value >= max || scope.value + step >= max) ) {
            setValue( max );
            return false;
          }
          
          // Re-set the value as an integer.
          setValue( scope.value );
        };
    }
  }
});