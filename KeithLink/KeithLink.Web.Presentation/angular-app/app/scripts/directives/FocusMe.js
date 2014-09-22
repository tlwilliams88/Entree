'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:focusMe
 * @description
 * sets focus to given element when focusMe attr is true
 */
angular.module('bekApp')
.directive('focusMe', function($timeout, $parse) {
  return {
    //scope: true,   // optionally create a child scope
    link: function(scope, element, attrs) {
      var model = $parse(attrs.focusMe);
      scope.$watch(model, function(value) {
        if(value === true) { 
          $timeout(function() {
            // element[0].focus(); 
            element[0].select();
          });
        }
      });
      // element.bind('blur', function() {
      //    scope.$apply(model.assign(scope, false));
      // });
    }
  };
});