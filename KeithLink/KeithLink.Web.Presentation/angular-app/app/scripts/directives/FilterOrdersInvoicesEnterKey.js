'use strict';

/*
 * Allows the use of the enter key to activate filter button 
 */

angular.module('bekApp')
.directive('enterKeyFilter', function(){
    return function (scope, element, attrs) {
        element.bind('keydown keypress keyup', function (event) {
            if(event.which === 13) {
                scope.$apply(function (){
                    scope.$eval(attrs.enterKeyFilter);
                });
 
                event.preventDefault();
            }
        });
    };
});