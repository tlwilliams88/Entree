'use strict';

angular.module('bekApp')
  .controller('CustomersController', ['$scope', 'customers',
    function ($scope, customers) {
    
    $scope.customers = customers;

  }]);
