'use strict';

angular.module('bekApp')
.controller('ReplicateListModalController', ['$scope', '$modalInstance', 'ListService', 'list', 'customers',
  function ($scope, $modalInstance, ListService, list, customers) {

  $scope.list = list;
  $scope.customers = customers;

  $scope.shareList = function(list, customers) {
    ListService.shareList(list, customers).then(function() {
      $modalInstance.close();
    });
  };

  $scope.copyList = function(list, customers) {
    ListService.copyList(list, customers).then(function() {
      $modalInstance.close();
    });
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

  $scope.selectedCustomers = [];
  $scope.toggleCustomerSelection = function(customer) {
    var idx = $scope.selectedCustomers.indexOf(customer);

    if (idx > -1) {
      $scope.selectedCustomers.splice(idx, 1);
    } else {
      $scope.selectedCustomers.push(customer);
    }
  };

}]);