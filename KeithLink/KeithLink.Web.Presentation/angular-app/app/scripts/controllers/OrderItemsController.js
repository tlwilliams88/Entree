'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$stateParams', 'CartService', 'OrderService', 'order',
  function ($scope, $stateParams, CartService, OrderService, order) {

  $scope.order = order;

  CartService.getShipDates();

  // $scope.isOrderChangeable = function() {
  //   if (order && order.requestedshipdate) {
  //     angular.forEach(CartService.shipDates, function(shipDate) {
  //       var requestedShipDateString = new Date(order.requestedshipdate).toDateString(),
  //         shipDateString = new Date(shipDate.shipdate + ' 00:00').toDateString();
  //       if (requestedShipDateString === shipDateString) {
  //         $scope.selectedShipDate = shipDate;
  //       }
  //     })
  //   }
  // };

  $scope.cancelOrder = function() {
    console.log('hit cancel order endpoint');
  };

}]);