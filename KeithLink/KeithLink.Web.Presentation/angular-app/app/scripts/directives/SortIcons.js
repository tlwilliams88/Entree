'use strict';

angular.module('bekApp')
  .directive('sortIcons', [function() {
    return {
      scope: {
        sortorder: '=',
        sortby: '=',
        field: '='
      },
      restrict: 'A',
      link: function (scope, element, attrs) {
        
      },
      templateUrl: 'views/directives/sorticons.html'
    };
  }]);