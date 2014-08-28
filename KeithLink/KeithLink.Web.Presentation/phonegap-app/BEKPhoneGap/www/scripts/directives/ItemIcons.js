'use strict';

angular.module('BEKPhoneGap')
  .directive('itemIcons', [function() {
    return {
      restrict: 'A',
      templateUrl: 'views/directives/itemicons.html'
    };
  }]);