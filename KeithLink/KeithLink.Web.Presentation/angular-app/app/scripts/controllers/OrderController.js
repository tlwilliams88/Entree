'use strict';

angular.module('bekApp')
.controller('OrderController', ['$scope', '$state', 'orders',
  function ($scope, $state, orders) {

  $scope.sortBy = 'createddate';
  $scope.sortOrder = true;

  $scope.orders = orders;


}]);