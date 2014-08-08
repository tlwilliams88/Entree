'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:pageHeaderBar
 * @description
 * loops through items in a list and creates the carousel slides for mobile devices
 * 
 * Inputs 
 * message: title or message displayed at the top of the page
 * this directive will also tranclude whatever is included within the directive tags
 */
angular.module('bekApp')
  .directive('pageHeaderBar', function() {
    return {
      restrict: 'A',
      replace : true,
      transclude: 'element',
      templateUrl: 'views/directives/headerBar.html',
      controller: function($scope) {
        $scope.$on('processing-start', function(event, args) {
          $scope.message = '';
        });

        $scope.$on('processing-end', function(event, args) {
          $scope.message = args.message;
        });
      }
    };
  })
  .directive('headerMessage', function() {
    return {
      restrict: 'A',
      transclude: true,
      replace: true,
      templateUrl: 'views/directives/headerMessage.html'
    };
  })
  .directive('headerButtons', function() {
    return  {
      restrict: 'A',
      transclude: true,
      replace: true,
      templateUrl: 'views/directives/headerButtons.html'
    };
  });