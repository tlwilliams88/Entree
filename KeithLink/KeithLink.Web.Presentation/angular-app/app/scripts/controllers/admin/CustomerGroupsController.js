'use strict';

angular.module('bekApp')
  .controller('CustomerGroupsController', ['$scope', 'CustomerGroupService', 'PagingModel',
    function (
      $scope, // angular
      CustomerGroupService, PagingModel // custom bek service
    ) {

  $scope.searchOptions = [{
    text: 'Customer Group Name',
    value: 'customergroup'
  },{
    text: 'User',
    value: 'user'
  },{
    text: 'Customer',
    value: 'customer'
  }];
  $scope.search = {};
  $scope.search.field = $scope.searchOptions[0].value;

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
    
  $scope.searchCustomerGroups = function(searchObj) {
    if (searchObj.field === 'customergroup') {
      customerGroupsPagingModel.filterData({ name: searchObj.term });
    } else {
      customerGroupsPagingModel.setAdditionalParams({
        terms: searchObj.term,
        type: searchObj.field
      });
      customerGroupsPagingModel.loadData();
    }
  };

    $scope.clearFilter = function(){   
     $scope.search.term = "";
     $scope.searchCustomerGroups($scope.search); 
    };

  $scope.infiniteScrollLoadMore = function() {
    customerGroupsPagingModel.loadMoreData($scope.customerGroups, $scope.totalGroups, $scope.loadingResults);
  };

}]);
