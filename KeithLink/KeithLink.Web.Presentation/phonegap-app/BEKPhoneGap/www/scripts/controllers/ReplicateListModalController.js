'use strict';

angular.module('bekApp')
.controller('ReplicateListModalController', ['$scope', '$modalInstance', 'ListService', 'list', 'customers',
  function ($scope, $modalInstance, ListService, list, customers) {

  $scope.list = list;
  $scope.customers = customers;
  $scope.selectedShareCustomers = [];
  $scope.selectedCopyCustomers = [];

  // select shared customers
  list.sharedwith.forEach(function(customerNumber) {
    customers.forEach(function(customer) {
      if (customerNumber === customer.customerNumber) {
        $scope.selectedShareCustomers.push(customer);
      }
    });
  });

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