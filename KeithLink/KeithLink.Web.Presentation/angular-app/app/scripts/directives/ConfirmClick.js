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
    restrict: 'A',
    link: function (scope, element, attr) {
      var clickAction = attr.confirmedClick;
      element.bind('click',function (event) {

        var msg;
        if (event.target.attributes['ng-confirm-click']) {
          msg = event.target.attributes['ng-confirm-click'].value;
        }
        if (!msg && event.target.parentElement.attributes['ng-confirm-click']) {
          msg = event.target.parentElement.attributes['ng-confirm-click'].value;
        }
        if (!msg) {
          msg = 'Are you sure?';
        }

        if ( window.confirm(msg) ) {
          scope.$eval(clickAction);
        }
      });
    }
  };
}]);