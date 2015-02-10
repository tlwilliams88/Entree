'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CustomerController
 * @description
 * # CustomerController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CustomerController', ['$scope', 'CustomerService',
    function($scope, CustomerService) {

  $scope.customersSortAsc = true;
  $scope.customersSortField = 'customerName';
  var customersConfig = {
    term: '',
    size: 30,
    from: 0,
    sortField: $scope.customersSortField,
    sortOrder: 'asc'
  };
  
  loadCustomers(customersConfig).then(setCustomers);

  function loadCustomers(customersConfig) {
    $scope.loadingCustomers = true;
    return CustomerService.getCustomers(
      customersConfig.term,
       customersConfig.size,
       customersConfig.from,
       customersConfig.sortField,
       customersConfig.sortOrder
    ).then(function(data) {
      $scope.loadingCustomers = false;
      $scope.totalCustomers = data.totalResults;
      return data.results;
    });
  }

  function setCustomers(customers) {
    $scope.customers = customers;
  }
  function appendCustomers(customers) {
    $scope.customers = $scope.customers.concat(customers);
  }

  $scope.searchCustomers = function (searchTerm) {
    customersConfig.from = 0;
    customersConfig.term = searchTerm;
    loadCustomers(customersConfig).then(setCustomers);
  };

  $scope.sortCustomers = function(field, order) {
    customersConfig.from = 0;
    customersConfig.size = 30;
    customersConfig.sortField = field;
    $scope.customersSortField = field;

    $scope.customersSortAsc = order;
    if (order) {
      customersConfig.sortOrder = 'asc';
    } else {
      customersConfig.sortOrder = 'desc';
    }
    
    loadCustomers(customersConfig).then(setCustomers);
  };

  $scope.infiniteScrollLoadMore = function() {
    if (($scope.customers && $scope.customers.length >= $scope.totalCustomers) || $scope.loadingCustomers) {
      return;
    }
    customersConfig.from += customersConfig.size;
    loadCustomers(customersConfig).then(appendCustomers);
  };

}]);
