'use strict';

angular.module('BEKPhoneGap')
  .directive('loadingSpinner', [function() {
    return {
      restrict: 'A',
      scope: {
        show: '=loadingSpinner'
      },
      templateUrl: 'views/directives/loadingspinner.html'
    };
  }]);