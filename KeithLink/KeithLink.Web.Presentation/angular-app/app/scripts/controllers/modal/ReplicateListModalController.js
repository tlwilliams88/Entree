'use strict';

/******
used to share and copy lists to other customers

takes 2 resolve values

list      : obj - list to share/copy
customers : array - list of customer objects the current user has access to share with
******/

angular.module('bekApp')
.controller('ReplicateListModalController', ['$scope', '$modalInstance', 'ListService', 'CustomerService', 'list',
  function ($scope, $modalInstance, ListService, CustomerService, list) {

  $scope.list = list;
  // $scope.customers = customers;
  $scope.selectedShareCustomers = [];
  $scope.selectedCopyCustomers = [];
  $scope.loadingResults = true;

  var customersStartingIndex = 0;
  var customersPerPage = 30;

  function loadCustomers(size, from) {
    $scope.loadingResults = true;
    return CustomerService.getCustomers('', size, from).then(function(data) {
      $scope.loadingResults = false;
      $scope.totalCustomers = data.totalResults;
      var customers = data.results;

      // select shared customers
      list.sharedwith.forEach(function(customerNumber) {
        customers.forEach(function(customer) {
          if (customerNumber === customer.customerNumber) {
            $scope.selectedShareCustomers.push(customer);
          }
        });
      });

      return customers;
    });
  }

  loadCustomers(customersPerPage, customersStartingIndex).then(function(customers) {
    $scope.customers = customers;
  });

  // INFINITE SCROLL
  $scope.infiniteScrollLoadMore = function() {
    if (($scope.customers && $scope.customers.length >= $scope.totalCustomers) || $scope.loadingResults) {
      return;
    }

    customersStartingIndex += customersPerPage;

    loadCustomers(customersPerPage, customersStartingIndex).then(function(customers) {
      $scope.customers = $scope.customers.concat(customers);
    });
  };

  $scope.shareList = function(list, customers) {
    ListService.shareList(list, customers).then(function() {
      var sharedwith = [];
      customers.forEach(function(customer) {
        sharedwith.push(customer.customerNumber);
      });
      $modalInstance.close(sharedwith);
    });
  };

  $scope.copyList = function(list, customers) {
    ListService.copyList(list, customers).then(function() {
      $modalInstance.close(list.sharedwith);
    });
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

  $scope.toggleCustomerSelection = function(customerList, customer) {
    var idx = customerList.indexOf(customer);

    if (idx > -1) {
      customerList.splice(idx, 1);
    } else {
      customerList.push(customer);
    }
  };

}]);