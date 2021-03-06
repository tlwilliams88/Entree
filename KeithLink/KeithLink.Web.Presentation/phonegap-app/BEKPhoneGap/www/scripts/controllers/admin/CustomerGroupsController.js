'use strict';

angular.module('bekApp')
  .controller('CustomerGroupsController', ['$scope', '$state', 'CustomerGroupService', 'PagingModel',
    function (
      $scope, // angular
      $state,
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
    customerGroupsPagingModel.setFilter();
    customerGroupsPagingModel.setAdditionalParams();

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
      $scope.search.term = '';
      customerGroupsPagingModel.clearFilters();
    };

  $scope.infiniteScrollLoadMore = function() {
    customerGroupsPagingModel.loadMoreData($scope.customerGroups, $scope.totalGroups, $scope.loadingResults);
  };

  $scope.goToInternalUser = function(email) {
    $scope.findInternalUserError = '';

    if (email.indexOf('@benekeith.com') === -1) {
      $scope.findInternalUserError = 'Please enter an internal email address.';
      return;
    }

    $state.go('menu.admin.editinternaluser', {email: email});
  };

}]);
