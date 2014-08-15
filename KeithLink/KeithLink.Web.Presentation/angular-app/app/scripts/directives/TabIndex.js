'use strict';

angular.module('bekApp')
.directive('setTabindex', function () {
	return {
		restrict: 'A',
		link: function (scope, elem, attr, ctrl) {
			attr.$set('tabindex', parseInt(attr.setTabindex, 10));
		}
	};
});