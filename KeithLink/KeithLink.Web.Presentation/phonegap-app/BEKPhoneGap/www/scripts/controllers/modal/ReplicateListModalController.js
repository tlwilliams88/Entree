'use strict';

/******
used to share and copy lists to other customers

takes 2 resolve values

list      : obj - list to share/copy
customers : array - list of customer objects the current user has access to share with
******/

angular.module('bekApp')
.controller('ReplicateListModalController', ['$scope', '$modalInstance', 'ListService', 'CustomerPagingModel', 'list',
  function ($scope, $modalInstance, ListService, CustomerPagingModel, list) {

  $scope.list = list;
  $scope.selectedShareCustomers = [];
  $scope.selectedCopyCustomers = [];
  $scope.loadingResults = true;

  function setCustomers(data) {
    $scope.totalCustomers = data.totalResults;

    // select shared customers
    list.sharedwith.forEach(function(customerNumber) {
      data.results.forEach(function(customer) {
        if (customerNumber === customer.customerNumber) {
          $scope.selectedShareCustomers.push(customer);
        }
      });
    });
    $scope.customers = data.results;
  }
  function appendCustomers(data) {
    $scope.customers = $scope.customers.concat(data.results);
  }
  function startLoading() {
    $scope.loadingResults = true;
  }
  function stopLoading() {
    $scope.loadingResults = false;
  }

  var customerPagingModel = new CustomerPagingModel(
    setCustomers,
    appendCustomers,
    startLoading,
    stopLoading,
    'customerName',
    false
  );

  customerPagingModel.loadCustomers();

  $scope.filterCustomerList = function (searchTerm) {
    customerPagingModel.filterCustomers(searchTerm);
  };
  $scope.infiniteScrollLoadMore = function() {
    customerPagingModel.loadMoreData($scope.customers, $scope.totalCustomers, $scope.loadingResults);
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