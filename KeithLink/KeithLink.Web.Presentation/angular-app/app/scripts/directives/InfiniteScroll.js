'use strict';

angular.module('bekApp')
.directive('whenScrolled', function($window) {
    return function(scope, elm, attr) {
        var raw = elm[0];
        var window = $window;

        angular.element(window).bind('scroll', function(e) {
        	var windowBottom = window.outerHeight + window.scrollY;
        	var elementBottom = raw.offsetHeight + 250;
            if (windowBottom > elementBottom) {
                scope.$apply(attr.whenScrolled);
            }
        });
    };
});