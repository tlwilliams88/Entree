'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$stateParams', 
  function ($scope, $stateParams) {

  if ($stateParams.orderId) {
    $scope.order = $stateParams.orderId;
  } else {
    $scope.order = 'none';
  }
  
}]);