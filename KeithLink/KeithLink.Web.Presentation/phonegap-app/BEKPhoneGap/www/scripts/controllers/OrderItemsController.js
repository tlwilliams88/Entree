'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$stateParams', 'order', 'OrderService',
  function ($scope, $stateParams, order, OrderService) {

  $scope.order = order;

}]);