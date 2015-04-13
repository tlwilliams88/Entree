'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:pageHeaderBar
 * @description
 * creates the page header bar with the page title at the top of most pages
 * 
 * Example

 <div page-header-bar>
  <div header-message>
    Item Details
  </div>
  <div header-buttons>
      <button class="btn btn-icon" ng-click="print()"><span class="icon-printer"></span></button>
  </div>
</div>
 */
angular.module('bekApp')
  .directive('pageHeaderBar', function() {
    return {
      restrict: 'A',
      replace : true,
      transclude: 'element',
      templateUrl: 'views/directives/headerBar.html'
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
      templateUrl: 'views/directives/headerButtons.html',
      controller: ['$scope', 'CartService', 'AccessService', function($scope, CartService, AccessService) {
        if (AccessService.isOrderEntryCustomer()) {
          $scope.cartHeaders = CartService.cartHeaders;
          if (CartService.cartHeaders.length === 0) {
            CartService.getCartHeaders();
          }
        }
      }]
    };
  });