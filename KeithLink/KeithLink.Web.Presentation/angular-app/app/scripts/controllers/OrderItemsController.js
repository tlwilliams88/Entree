'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$stateParams', 'CartService', 'OrderService', 'order',
  function ($scope, $stateParams, CartService, OrderService, order) {

  $scope.order = order;

}]);