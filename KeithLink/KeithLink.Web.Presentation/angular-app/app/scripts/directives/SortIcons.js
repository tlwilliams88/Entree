'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:sortIcons
 * @description
 * provides the sort arrow icons for table headers
 *
 * used for lists, orders, and search results
 */
angular.module('bekApp')
  .directive('sortIcons', [function() {
    return {
      replace: true,
      scope: {
        sortorder: '=',
        sortby: '=',
        field: '='
      },
      restrict: 'A',
      // link: function (scope, element, attrs) {
      //   scope.sortorder = attrs.sortorder;
      //   scope.sortby = attrs.sortby;
      //   scope.field = attrs.field;
      // },
      templateUrl: 'views/directives/sorticons.html'
    };
  }]);