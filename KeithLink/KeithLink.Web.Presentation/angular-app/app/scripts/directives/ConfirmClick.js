'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:ngConfirmClick
 * @description
 * displays confirmation message and executes the given function if message is approved
 *
 * used when deleting lists and orders
 */
angular.module('bekApp')
.directive('ngConfirmClick', [ function(){
    return {
        link: function (scope, element, attr) {
            var msg = attr.ngConfirmClick || 'Are you sure?';
            var clickAction = attr.confirmedClick;
            element.bind('click',function (event) {
                if ( window.confirm(msg) ) {
                    scope.$eval(clickAction);
                }
            });
        }
    };
}]);