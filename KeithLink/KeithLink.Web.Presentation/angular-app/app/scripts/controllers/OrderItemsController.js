'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$stateParams', 'CartService', 'OrderService', 'order',
  function ($scope, $stateParams, CartService, OrderService, order) {

  $scope.order = order;

  // CartService.getShipDates().then(function() {
  //   var shipDateObject = CartService.findCutoffDate(order);
  //   if (shipDateObject) {
  //     $scope.cutoffdate = shipDateObject.cutoffdatetime;
  //   }
  // });

  $scope.cancelOrder = function() {
    console.log('hit cancel order endpoint');
  };

}]);