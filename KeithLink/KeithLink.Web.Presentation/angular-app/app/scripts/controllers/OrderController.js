'use strict';

angular.module('bekApp')
.controller('OrderController', ['$scope', '$state', 'orders',
  function ($scope, $state, orders) {

  $scope.orders = orders;


}]);