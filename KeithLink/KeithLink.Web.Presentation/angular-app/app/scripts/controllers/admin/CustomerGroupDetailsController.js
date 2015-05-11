'use strict';

angular.module('bekApp')
  .controller('CustomerGroupDetailsController', ['$scope', '$state', '$stateParams', '$filter', 'originalCustomerGroup', 'CustomerGroupService', 'CustomerPagingModel', 'UserProfileService',
    function ($scope, $state, $stateParams, $filter, originalCustomerGroup, CustomerGroupService, CustomerPagingModel, UserProfileService) {

  if ($stateParams.groupId === 'new') {
    $scope.originalCustomerGroup = {
      customers: [],
      adminusers: []
    };
    $scope.isNew = true;
  } else {
    $scope.originalCustomerGroup = originalCustomerGroup;
    $scope.isNew = false;
  }
  $scope.customerGroup = angular.copy($scope.originalCustomerGroup);
  $scope.dirty = false;

  /**********
  CUSTOMERS
  **********/

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

  customerPagingModel.loadCustomers();
  
  function findSelectedCustomers(customers) {
    // check if customer is selected
    customers.forEach(function(customer) {
      $scope.customerGroup.customers.forEach(function(profileCustomer) {
        if (customer.customerId === profileCustomer.customerId) {
          customer.selected = true;
        }
      });

      if (!customer.selected) {
        customer.selected = false;
        customer.isChecked = false;
      }
    });
    return customers;
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

  $scope.searchCustomers = function (searchTerm) {
    customerPagingModel.filterCustomers(searchTerm);
  };

  $scope.sortCustomers = function(field, sortDescending) {
    $scope.customersSortDesc = sortDescending;
    $scope.customersSortField = field;
    customerPagingModel.sortCustomers(field, sortDescending);
  };

  $scope.infiniteScrollLoadMore = function() {
    customerPagingModel.loadMoreData($scope.customers, $scope.totalCustomers, $scope.loadingCustomers);
  };

  $scope.selectCustomer = function(customer) {
   $scope.customerGroup.customers.push(customer);
  customer.selected = true;
  };

  $scope.addSelected = function(){
    $scope.customerGroupDetailsForm.$setDirty();
    $scope.customers.forEach(function(customer) {
      if(customer.isChecked){    
        customer.isChecked = false;
        customer.selected = true;
        $scope.customerGroup.customers.push(angular.copy(customer));  
      }
    })    
      if($scope.filteredCustomers.length<30 || $scope.allAvailableSelected ){
        $scope.infiniteScrollLoadMore();
      }
       $scope.allAvailableSelected = $scope.allRemovableSelected = false;
  };

  $scope.selectAll = function(allSelected, source){
    if(source=='add'){
      $scope.customers.forEach(function(customer) {
        if(customer.selected == false){
       customer.isChecked = allSelected;
     }
      })
    }
    if(source=='remove'){
      $scope.customerGroup.customers.forEach(function(availableCustomer) {
        availableCustomer.isChecked = allSelected;
      })
    }
  };

 $scope.unselectCustomer = function(customer) {
  var idx = $scope.customerGroup.customers.indexOf(customer);   
  $scope.customerGroup.customers.splice(idx, 1);
  $scope.customers.forEach(function(availableCustomer) {  
   if (customer.customerNumber === availableCustomer.customerNumber) {    
     availableCustomer.selected = false;
    }    
  });    
  customer.selected = false;
   };

  $scope.removeSelected = function(selectedCustomer) {
    $scope.customerGroupDetailsForm.$setDirty();
    $scope.foundMatch = false;
    $scope.customerGroup.customers.forEach(function(availableCust){
        if(availableCust.isChecked){   
            $scope.customers.forEach(function(customer) {
                if (availableCust.customerNumber === customer.customerNumber) {
                  $scope.foundMatch = true;                                  
                  customer.selected  = customer.isChecked = false;
                  availableCust.selected  = availableCust.isChecked = false;
                }
            });
            if(!$scope.foundMatch){
              $scope.unselectCustomer(selectedCustomer);
            } 
        }  
    })
     $scope.customerGroup.customers = $filter('filter')($scope.customerGroup.customers, {selected: 'true'});
     $scope.allAvailableSelected = $scope.allRemovableSelected = false;  
  };


    $scope.customerSearchTerm = ''; 
   $scope.clearFilter = function(){ 
      $scope.customerSearchTerm = ''; 
      $scope.searchCustomers($scope.customerSearchTerm); 
    };


  /**********
  USERS
  **********/

  $scope.addUserError = '';
  $scope.checkUserAndAddAdmin = function(emailAddress) {
    if(emailAddress){$scope.dirty = true;}    
    var isDuplicateUser = false;
    $scope.addUserError = '';

    // check if user is already in list of selected users
    $scope.customerGroup.adminusers.forEach(function(user) {
      
      if (user.emailaddress === emailAddress) {
        isDuplicateUser = true;
      }
    });

    if (isDuplicateUser) {
      $scope.addUserError = 'This user is already an admin on this customer group.';
      return;
    }

    // check if user exists in the database
    var data = {
      email: emailAddress
    };
    UserProfileService.getAllUsers(data).then(function (profiles) {
      if (profiles.length === 1) {
        $scope.customerGroup.adminusers.push(profiles[0]);
        $scope.submitForm($scope.customerGroup);
      } else {
        // display error message to user
        UserProfileService.createUserFromAdmin(data).then(function (profiles) {
          if (profiles.length === 1) {
            $scope.customerGroup.adminusers.push(profiles[0]);
            $scope.submitForm($scope.customerGroup);
          } else {
            // display error message to user
          }
        });
      }
    });
  };

  $scope.removeUser = function(user) {
    $scope.dirty = true;
    var idx = $scope.customerGroup.adminusers.indexOf(user);
    $scope.customerGroup.adminusers.splice(idx, 1);
    $scope.submitForm($scope.customerGroup);
  };

  $scope.goBack = function(){
 $state.go('menu.admin.customergroup');
  };

  /***********
  FORM EVENTS
  ***********/
  var processingCreateCustomerGroup = false;
  function createNewCustomerGroup(group) {
    if (!processingCreateCustomerGroup) {
      processingCreateCustomerGroup = true;
      CustomerGroupService.createGroup(group).then(function(newGroup) {
        $scope.displayMessage('success', 'Successfully created a new customer group.');
        $state.go('menu.admin.customergroupdetails', { groupId: newGroup.id });
      }, function(error) {
        $scope.displayMessage('error', 'Error creating new customer group.');
      }).finally(function() {
        processingCreateCustomerGroup = false;
      });
    }
  }

  var processingSaveCustomerGroup = false;
  function saveCustomerGroup(group) {
    if (!processingSaveCustomerGroup) {
      processingSaveCustomerGroup = true;
      delete group.customerusers;
      CustomerGroupService.updateGroup(group).then(function(groups) {
        $scope.displayMessage('success', 'Successfully saved customer group.');
      }, function(error) {
        var message = error ? error : 'Error updating customer group.';
        $scope.displayMessage('error', message);
      }).finally(function() {
        processingSaveCustomerGroup = false;
      });
    }
  }

  $scope.submitForm = function(group) {
    $scope.customerGroupDetailsForm.$setPristine();

    if ($scope.isNew) {
      createNewCustomerGroup(group);
    } else {
      saveCustomerGroup(group);
    }
  };

  $scope.deleteGroup = function(id) {
    CustomerGroupService.deleteGroup(id).then(function() {
      $scope.customerGroupDetailsForm.$setPristine();
      $state.go('menu.admin.customergroup');
      $scope.displayMessage('success', 'Successfully deleted customer group.');
    }, function() {
      $scope.displayMessage('error', 'Error deleting customer group.');
    })
  };

}]);
