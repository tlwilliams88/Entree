'use strict';

angular.module('bekApp')
  .directive('itemIcons', [function() {
    return {
      restrict: 'A',
      scope: {
        item: '=itemIcons'
      },
      templateUrl: 'views/directives/itemicons.html'
    };
  }]);