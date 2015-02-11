'use strict';

angular.module('bekApp')
.controller('OrderController', ['$scope', '$state', '$timeout', '$modal', 'OrderService', 'PagingModel',
  function ($scope, $state, $timeout, $modal, OrderService, PagingModel) {

  var currentCustomer = $scope.selectedUserContext.customer;

  function setOrders(data) {
    $scope.orders = data.results;
    $scope.totalOrders = data.totalResults;
  }
  function appendOrders(data) {
    $scope.orders = $scope.orders.concat(data.results);
  }
  function startLoading() {
    $scope.loadingResults = true;
  }
  function stopLoading() {
    $scope.loadingResults = false;
  }

  $scope.sort = {
    field: 'createddate',
    sortDescending: true
  };

  var ordersPagingModel = new PagingModel( 
    OrderService.getOrders, 
    setOrders,
    appendOrders,
    startLoading,
    stopLoading,
    $scope.sort
  );

  ordersPagingModel.loadData();
    
  $scope.filterOrders = function(filterFields) {
    ordersPagingModel.filterData(filterFields);
  };
  $scope.clearFilters = function() {
    $scope.filterFields = {};
    ordersPagingModel.clearFilters();
  };
  $scope.infiniteScrollLoadMore = function() {
    ordersPagingModel.loadMoreData($scope.orders, $scope.totalOrders, $scope.loadingResults);
  };
  $scope.sortOrders = function(field, sortDescending) {
    $scope.sort = {
      field: field,
      sortDescending: sortDescending
    };
    ordersPagingModel.sortData($scope.sort);
  };

  var data = { response: {}, calls: 0 };
  var poller = function() {
    OrderService.pollOrderHistory().then(function(response) {
      // console.log('poll');
      data.calls++;
      
      if (currentCustomer.lastOrderUpdate === response.lastupdated) {
        // no update made, keep polling

        if (data.calls > 5) {
          // stop polling after x-number calls
          // $timeout.cancel(colorRowTimer);
          $scope.displayMessage('error', 'No updates made. Try again later.');
        } else {
          // keep polling
          $timeout(poller, 2000);
        }

      } else {
        $scope.displayMessage('success', 'Successfully received lastest order updates.');
        ordersPagingModel.pageIndex = 0;
        ordersPagingModel.filter = [];
        ordersPagingModel.loadData();

      }
    });      
  };

  $scope.refreshOrderHistory = function() {
    OrderService.refreshOrderHistory().then(function(response) {
      poller();  
    });
  };

  $scope.openExportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
        headerText: function () {
          return 'Orders';
        },
        exportMethod: function() {
          return OrderService.exportOrders;
        },
        exportConfig: function() {
          return OrderService.getOrderExportConfig();
        },
        exportParams: function() {
          return null;
        }
      }
    });
  };
  
}]);