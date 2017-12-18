'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:stopPageScroll
 * @description
 * displays confirmation message and executes the given function if message is approved
 *
 * used when deleting lists and orders
 */
angular.module('bekApp')
.directive('stopPageScroll', ['$timeout', function($timeout){
    return {
    restrict: 'A',
        link: function (scope, element, attr) {
            $timeout(function() {
                $('.nav, .select2-results').on('mousewheel DOMMouseScroll',function (event) {
                    var d = event.originalEvent.wheelDelta || -event.originalEvent.detail,
                    dir = d > 0 ? 'up' : 'down',
                    stop = (dir == 'up' && this.scrollTop == 0) || 
                         (dir == 'down' && this.scrollTop >= this.scrollHeight-this.offsetHeight);
                    stop && event.preventDefault();
                });
            }, 500);
        }
    };
}]);