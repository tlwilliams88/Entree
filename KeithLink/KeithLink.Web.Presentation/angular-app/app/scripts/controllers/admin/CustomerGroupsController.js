'use strict';

angular.module('bekApp')
  .controller('CustomerGroupsController', ['$scope', 'CustomerGroupService', 'PagingModel',
    function (
      $scope, // angular
      CustomerGroupService, PagingModel // custom bek service
    ) {

  function setGroups(data) {
    $scope.customerGroups = data.results;
    $scope.totalGroups = data.totalResults;
  }
  function appendGroups(data) {
    $scope.customerGroups = $scope.customerGroups.concat(data.results);
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

  var customerGroupsPagingModel = new PagingModel( 
    CustomerGroupService.getGroups, 
    setGroups,
    appendGroups,
    startLoading,
    stopLoading,
    sort 
  );

  customerGroupsPagingModel.loadData();
    
  $scope.searchCustomerGroups = function(searchTerm) {
    customerGroupsPagingModel.filterData({ name: searchTerm });
  };

  $scope.infiniteScrollLoadMore = function() {
    customerGroupsPagingModel.loadMoreData($scope.customerGroups, $scope.totalGroups, $scope.loadingResults);
  };

}]);
