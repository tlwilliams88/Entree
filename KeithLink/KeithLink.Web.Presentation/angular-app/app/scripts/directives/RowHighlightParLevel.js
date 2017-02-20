'use strict';

angular.module('bekApp')
  .directive('rowHighlight', ['$filter', '$window', function($filter, $window, ListService) {
    return {
      restrict: 'A',
      require: ['^form','ngModel'],
      scope: { 
        value: '=ngModel',
      },
        
      link: function( $scope, element, attributes ) {
          
    element.bind('focus', function (event) {
            $('.ATOrowHighlight').removeClass('ATOrowHighlight');
            $(event.target).parents('tr').addClass('ATOrowHighlight');
        });
      }
    };
}]);