'use strict';

angular.module('bekApp')
.controller('OrderController', ['$scope', '$state', 'orders',
  function ($scope, $state, orders) {

  $scope.orders = orders;

  if (orders && $state.current.name === 'menu.order') {
    $state.go('menu.order.items', { orderId: orders[0].id });
  }

}]);