'use strict';

angular.module('bekApp')
  .controller('CustomerGroupsController', ['$scope', 'CustomerGroupService',
    function (
      $scope, // angular
      CustomerGroupService // custom bek service
    ) {

  function loadCustomerGroups(params) {
    $scope.loadingResults = true;
    return CustomerGroupService.getGroups(params).then(function(data) {
      $scope.loadingResults = false;
      $scope.totalGroups = data.totalResults;
      
      return data.results;
    });
  }
    
  $scope.searchCustomerGroups = function(searchTerm) {
    $scope.params.filter = {
      field: 'name',
      value: searchTerm
    };
    loadCustomerGroups($scope.params).then(function(groups) {
      $scope.customerGroups = groups;
    });
  };

  $scope.infiniteScrollLoadMore = function () {
    if (($scope.customerGroups && $scope.customerGroups.length >= $scope.totalGroups) || $scope.loadingResults) {
      return;
    }

    $scope.params.from += $scope.params.size;

    loadCustomerGroups($scope.params).then(function (customerGroups) {
      $scope.customerGroups = $scope.customerGroups.concat(customerGroups);
    });
  };

  $scope.params = {
    size: 30,
    from: 0,
    sort: [{
      field: 'name',
      order: 'asc'
    }]
  };

  loadCustomerGroups($scope.params).then(function(groups) {
    $scope.customerGroups = groups;
  });

}]);
