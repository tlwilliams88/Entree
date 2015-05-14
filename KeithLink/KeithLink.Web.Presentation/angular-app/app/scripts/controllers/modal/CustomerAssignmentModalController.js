'use strict';

angular.module('bekApp')
.controller('CustomerAssignmentModalController', ['$scope', '$modalInstance', 'CustomerPagingModel', 'customerGroupId', 'selectedCustomers',
  function ($scope, $modalInstance, CustomerPagingModel, customerGroupId, selectedCustomers) {

  function findSelectedCustomers(customers) {
    var unselectedCustomers = [];
    // check if customer is selected
    customers.forEach(function(customer) {
      var isCustomerSelected = false;

      selectedCustomers.forEach(function(profileCustomer) {
        if (customer.customerId === profileCustomer.customerId) {
          isCustomerSelected = true;
        }
      });
      if (isCustomerSelected === false) {
        unselectedCustomers.push(customer);
      }
    });
    return unselectedCustomers;
  }

  function setCustomers(data) {
    $scope.customers = findSelectedCustomers(data.results);
    $scope.totalCustomers = data.totalResults;
  }
  function appendCustomers(data) {
    $scope.customers = $scope.customers.concat(findSelectedCustomers(data.results));
  }
  function startLoading() {
    $scope.loadingCustomers = true;
  }
  function stopLoading() {
    $scope.loadingCustomers = false;
  }

  $scope.selectedCount = 0;
  $scope.customerSearchTerm = '';
  $scope.customersSortDesc = false;
  $scope.customersSortField = 'customerName';

  var customerPagingModel = new CustomerPagingModel(
    setCustomers,
    appendCustomers,
    startLoading,
    stopLoading,
    $scope.customersSortField,
    $scope.customersSortDesc
  );

  if (customerGroupId) {
    customerPagingModel.accountId = customerGroupId;
  }

  customerPagingModel.loadCustomers();

  $scope.updateCount = function(isSelected) {
    if (isSelected === true) {
      $scope.selectedCount++;
    } else {
      $scope.selectedCount--;
    }
  };

  $scope.selectAll = function(allSelected){
    $scope.customers.forEach(function(customer) {
      customer.selected = allSelected;
    });

    if (allSelected === true) {
      $scope.selectedCount = $scope.customers.length;
    } else {
      $scope.selectedCount = 0;
    }
  }; 

  $scope.selectCustomer = function(customer) {
    customer.selected = !customer.selected;
    $scope.updateCount(customer.selected);
  };

  $scope.stopEvent = function(e) {
    e.stopPropagation();
  };

  $scope.searchCustomers = function (searchTerm) {
    customerPagingModel.filterCustomers(searchTerm);
  };

  $scope.clearFilter = function() {
    $scope.customerSearchTerm = ''; 
    $scope.searchCustomers($scope.customerSearchTerm); 
  };

  $scope.sortCustomers = function(field, sortDescending) {
    $scope.customersSortDesc = sortDescending;
    $scope.customersSortField = field;
    customerPagingModel.sortCustomers(field, sortDescending);
  };

  $scope.infiniteScrollLoadMore = function() {
    customerPagingModel.loadMoreData($scope.customers, $scope.totalCustomers, $scope.loadingCustomers);
  };


  /* ******
  * FORM EVENTS
  * *******/
  $scope.addSelectedCustomers = function(customers) {
    var selectedCustomers = [];
    customers.forEach(function(customer) {
      if (customer.selected === true) {
        delete customer.selected;
        selectedCustomers.push(customer);
      }
    });
      
    $modalInstance.close(selectedCustomers);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);