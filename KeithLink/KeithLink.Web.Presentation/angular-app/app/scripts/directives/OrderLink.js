'use strict';

angular.module('bekApp')
  .directive('orderLink', [function() {
    return {
      restrict: 'A',
      templateUrl: 'views/directives/orderlink.html'
    };
  }]);