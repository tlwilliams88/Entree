'use strict';

angular.module('bekApp')
  .directive('loadingSpinner', [function() {
    return {
      restrict: 'A',
      scope: {
        show: '=loadingSpinner'
      },
      templateUrl: 'views/directives/loadingspinner.html'
    };
  }]);