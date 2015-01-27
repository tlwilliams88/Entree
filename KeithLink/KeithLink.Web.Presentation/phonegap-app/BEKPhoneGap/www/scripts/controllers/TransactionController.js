'use strict';

angular.module('bekApp')
  .controller('TransactionController', ['$scope', 'TransactionService', 'Constants', 'LocalStorage', '$state',
    function ($scope, TransactionService, Constants, LocalStorage, $state) {

  function loadTransactions(params) {
    var promise;
    $scope.loadingResults = true;

    params.filter = createFilterObject();

    return TransactionService.getPendingTransactions(params).then(function(data) {
      $scope.loadingResults = false;
      $scope.totalTransactions = data.totalResults;

      return data.results;
    });
  }

  function setTransactions(transactions) {
    $scope.transactions = transactions;
  }

  /**********
  PAGING, SORTING, FILTERING
  **********/

  function createFilterObject() {

    // create array of filter fields
    // example filter object
    // filter: {
    //   field: 'subject', // this contains the info for the first filter
    //   value: 'value',
    //   filter: [ // use this array if filtering by more than one field
    //      {
    //        field: 'name',
    //        value: 'value'
    //      }
    //   ]
    // }

    var filterObject = {
      // field: 
      // value:
      filter: []
    };

    // if filter row is used
    if ($scope.filterRowFields) {
      // var filterList = [];
      for (var propertyName in $scope.filterRowFields) {
        if ($scope.filterRowFields[propertyName] && $scope.filterRowFields[propertyName] !== '') {
          var filterField = {
            field: propertyName,
            value: $scope.filterRowFields[propertyName]
          };
          filterObject.filter.push(filterField);
        }
      }
    }

    // if no filters are selected, remove filter object
    if (filterObject.filter.length === 0) {
      filterObject = [];
    
    
    // otherwise, pop first filter
    } else {
      var firstFilter = filterObject.filter.splice(0, 1)[0];
      filterObject.field = firstFilter.field;
      filterObject.value = firstFilter.value;
      if (firstFilter.type) {
        filterObject.type = firstFilter.type;
      }
    }

    return filterObject;
  }

  $scope.filterTransactions = function (filterFields) {
    $scope.transactionParams.from = 0;
    loadTransactions($scope.transactionParams).then(setTransactions);
  };

  $scope.clearFilters = function () {
    $scope.filterRowFields = {};
    loadTransactions($scope.transactionParams).then(setTransactions);
  };

  $scope.sortTransactions = function (field, order) {
    $scope.sortOrder = order;

    $scope.transactionParams.sort = [{
      field: field,
      order: order ? 'desc' : 'asc'
    }];

    $scope.transactionParams.from = 0;
    loadTransactions($scope.transactionParams).then(setTransactions);
  };

  $scope.infiniteScrollLoadMore = function () {
    if (($scope.transactions && $scope.transactions.length >= $scope.totalTransactions) || $scope.loadingResults) {
      return;
    }

    $scope.transactionParams.from += $scope.transactionParams.size;

    loadTransactions($scope.transactionParams).then(function (transactions) {
      $scope.transactions = $scope.transactions.concat(transactions);
    });
  };

  $scope.sortOrder = true;
  $scope.transactionParams = {
    size: Constants.infiniteScrollPageSize,
    from: 0
    // sort: [{
    //   field: 'messagecreatedutc',
    //   order: 'desc'
    // }]
    // filter: {
    //   field: 'subject',
    //   value: 'value',
    //   filter: [
    //      {
    //        field: 'name',
    //        value: 'value'
    //      }
    //   ]
    // }
  };

  // set context to all customers
  var tempContext = {
    text: 'All Customers'
  };
  $scope.setSelectedUserContext(tempContext);

  loadTransactions($scope.transactionParams).then(setTransactions);


}]);
