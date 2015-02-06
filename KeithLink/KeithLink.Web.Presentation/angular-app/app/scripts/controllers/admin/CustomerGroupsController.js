'use strict';

angular.module('bekApp')
  .controller('CustomerGroupsController', ['$scope', 'CustomerGroupService', 'PagingModel',
    function (
      $scope, // angular
      CustomerGroupService, PagingModel // custom bek service
    ) {

  function setGroups(groups) {
    $scope.customerGroups = groups;
  }
  function appendGroups(groups) {
    setGroups($scope.customerGroups.concat(groups));
  }
  function setTotal(total) {
    $scope.totalGroups = total;
  }
  function startLoading() {
    $scope.loadingResults = true;
  }
  function stopLoading() {
    $scope.loadingResults = false;
  }

  var sort = {
    field: 'name',
    sortDescending: false
  };

  var customerGroups = new PagingModel( 
    CustomerGroupService.getGroups, 
    setGroups,
    appendGroups,
    setTotal,
    startLoading,
    stopLoading,
    sort 
  );

  customerGroups.loadData();
    
  $scope.searchCustomerGroups = function(searchTerm) {
    customerGroups.filterData({ name: searchTerm });
  };

  $scope.infiniteScrollLoadMore = function() {
    customerGroups.loadMoreData($scope.customerGroups, $scope.totalGroups, $scope.loadingResults);
  };

}]);
