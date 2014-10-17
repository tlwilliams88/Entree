'use strict';

angular.module('bekApp')
  .directive('itemIcons', [function() {
    return {
      restrict: 'A',
      templateUrl: 'views/directives/itemicons.html'
    };
  }]);