'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$stateParams', 'OrderService', 'order',
  function ($scope, $stateParams, OrderService, order) {

  $scope.order = order;
  
}]);