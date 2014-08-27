'use strict';

angular.module('BEKPhoneGap')
.directive('goToNextParlevel', function () {
	return {
		restrict: 'A',
		link: function (scope, elem, attr, ctrl) {
			
      elem.bind('keyup', function(event) {
        if(event.keyCode === 40 || event.keyCode === 13) {
		      var tabindex = parseInt(angular.element(event.target).attr('tabindex')) + 1;

          // get the parlevel input with the new tabindex
		      var elems = angular.element('[tabindex=' + tabindex + '][name=parlevel]');
		      if (elems.length === 1) {
		        elems[0].focus();
		      } else { // go back to the beginning of the list
		        angular.element('[name=parlevel]')[0].focus();
		      }
	    	}
	    });
		}
	};
});