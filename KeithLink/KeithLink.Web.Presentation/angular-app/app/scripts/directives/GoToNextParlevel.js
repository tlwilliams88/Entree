'use strict';

angular.module('bekApp')
.directive('goToNextParlevel', function () {
	return {
		restrict: 'A',
		link: function (scope, elem, attr, ctrl) {
			
      elem.bind('keyup', function(event) {
        var tabindex = parseInt(angular.element(event.target).attr('tabindex'));
        var endIndex;

        if(event.keyCode === 40 || event.keyCode === 13) {
          // 13-enter, 40-down arrow goes to next par level
		      tabindex++;
          endIndex = 0;
          focusParlevelInput(tabindex, endIndex);
        } else if (event.keyCode === 38) {
          // 38-up arrow, go to previous parlevel
          tabindex--;
          endIndex = angular.element('[name=parlevelForm] [name=parlevel]').length - 1;
          focusParlevelInput(tabindex, endIndex);
        }

	    });

      function focusParlevelInput(tabindex, endIndex) {
        // get the parlevel input with the new tabindex
        // if next tabindex does not exist go to end index
        var elems = angular.element('[tabindex=' + tabindex + '][name=parlevel]');
        if (elems.length === 1) {
          elems[0].focus();
        } else { // go back to the beginning of the list
          angular.element('[name=parlevelForm] [name=parlevel]')[endIndex].focus();
        }
      }
		}
	};
});