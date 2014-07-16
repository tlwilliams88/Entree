'use strict';

angular.module('bekApp')
  .directive('pageHeaderBar', function () {
    
    return {
      restrict: 'A',
      transclude: true,
      scope: {
        pageTitle: '=message'
      },
      templateUrl: 'views/directives/pageheaderbar.html'
    };

  });