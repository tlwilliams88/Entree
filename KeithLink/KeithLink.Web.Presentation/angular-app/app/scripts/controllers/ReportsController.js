'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ReportsController
 * @description
 * # ReportsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ReportsController', ['$scope', '$state', 'CartService', 'ENV',
    function ($scope, $state, CartService, ENV) {

   	 CartService.getCartHeaders().then(function(cartHeaders){
      $scope.cartHeaders = cartHeaders;
    });
    
    // auto-redirect user to item usage report if they don't have access to kbit
    // if (!$scope.userProfile.iskbitcustomer) {
    //   $state.go('menu.itemusagereport');
    // }
}]);
