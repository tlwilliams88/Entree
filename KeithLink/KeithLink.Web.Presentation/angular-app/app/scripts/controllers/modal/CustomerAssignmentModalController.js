'use strict';

angular.module('bekApp')
.controller('CustomerAssignmentModalController', ['$scope', '$filter', '$modalInstance', 'CustomerPagingModel', 'customerGroupId', 'selectedCustomers',
  function ($scope, $filter, $modalInstance, CustomerPagingModel, customerGroupId, selectedCustomers) {

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
    setSelected();  

  }

  function setSelected(){
    var idx =0;
    if($scope.customers){
      $scope.customers.forEach(function(customer){
        if($filter('filter')($scope.selectedCustStorage, {customerId: customer.customerId}).length != 0){
          $scope.customers[idx].selected = true;
        }
        idx++;
      });
    }
}
  function updateStoredCustomers(customer){
    if(customer.selected){
    if($filter('filter')($scope.selectedCustStorage, {customerId: customer.customerId}).length === 0){
      $scope.selectedCustStorage.push(customer);
    }
  }else{
      var idx=0;
      $scope.selectedCustStorage.forEach(function(selectedCust){
        if(customer.customerId == selectedCust.customerId){
          $scope.selectedCustStorage.splice(idx,1);
        }
        idx++;
      });
    }
 }

  function appendCustomers(data) {
    $scope.customers = $scope.customers.concat(findSelectedCustomers(data.results));
  }
  function startLoading() {
    $scope.loadingCustomers = true;
  }
  function stopLoading() {
    $scope.loadingCustomers = false;
    if($scope.customers.length <30){$scope.infiniteScrollLoadMore();}
  }

  $scope.selectedCount = 0;
  $scope.customerSearchTerm = '';
  $scope.customersSortDesc = false;
  $scope.customersSortField = 'customerName';
  $scope.selectedCustStorage=[];

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
     
      $scope.customers.forEach(function(customer){
        if($filter('filter')($scope.selectedCustStorage, {customerId: customer.customerId}).length === 0){
            $scope.selectedCustStorage.push(customer);
          }
      }); 
      $scope.selectedCount = $scope.selectedCustStorage.length;
    } else { 
        $scope.customers.forEach(function(customer){  
        $scope.selectedCustStorage.forEach(function(selectedCustomer){
          if(customer.customerId === selectedCustomer.customerId){
           selectedCustomer.isUnchecked = true;
          }   
        });
    });
        var customersToKeep=[];
        $scope.selectedCustStorage.forEach(function(customer){
          if(customer.isUnchecked){
            delete customer.isUnchecked;            
          }
          else{
            customersToKeep.push(customer);
          }
        });
     $scope.selectedCustStorage = customersToKeep;
     $scope.selectedCount = $scope.selectedCustStorage.length;
      }
  }; 

  $scope.selectCustomer = function(customer) {
    customer.selected = !customer.selected;
    $scope.updateCount(customer.selected);
    updateStoredCustomers(customer);
  };

  $scope.stopEvent = function(e, customer) {
    e.stopPropagation();
    updateStoredCustomers(customer);
  };

  $scope.searchCustomers = function (searchTerm) {
    customerPagingModel.filterCustomers(searchTerm);
    $scope.allAvailableSelected = false;
  };

  $scope.clearFilter = function() {
    $scope.customerSearchTerm = ''; 
    $scope.searchCustomers($scope.customerSearchTerm); 
    setSelected(); 
  };

  $scope.sortCustomers = function(field, sortDescending) {
    $scope.customersSortDesc = sortDescending;
    $scope.customersSortField = field;
    customerPagingModel.sortCustomers(field, sortDescending);
  };

  $scope.infiniteScrollLoadMore = function() {
    customerPagingModel.loadMoreData($scope.customers, $scope.totalCustomers, $scope.loadingCustomers);
    setSelected();
  };


  /* ******
  * FORM EVENTS
  * *******/
  $scope.addSelectedCustomers = function() {
    var selectedCustomers = [];
    $scope.selectedCustStorage.forEach(function(customer) {      
        delete customer.selected;
        selectedCustomers.push(customer);      
    });      
    $modalInstance.close(selectedCustomers);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);