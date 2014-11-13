'use strict';

angular.module('bekApp')
.controller('ReplicateListModalController', ['$scope', '$modalInstance', 'ListService', 'list', 'customers',
  function ($scope, $modalInstance, ListService, list, customers) {

  $scope.list = list;
  $scope.customers = customers;

  $scope.shareList = function(listId, customerNumber) {
    ListService.shareList(listId, customerNumber);
    $modalInstance.close();
  };

  $scope.copyList = function(listId, customerNumber) {
    ListService.copyList(listId, customerNumber);
    $modalInstance.close();
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);