'use strict';

angular.module('bekApp')
  .controller('CustomerGroupDetailsController', ['$scope', '$state', '$stateParams', 'originalCustomerGroup', 'CustomerGroupService', 'CustomerPagingModel', 'UserProfileService',
    function ($scope, $state, $stateParams, originalCustomerGroup, CustomerGroupService, CustomerPagingModel, UserProfileService) {
    //comment
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

  /**********
  USERS
  **********/
  $scope.checkUserAndAddAdmin = function(emailAddress) {
    var isDuplicateUser = false;

    // check if user is already in list of selected users
    $scope.customerGroup.adminusers.forEach(function(user) {
      
      if (user.emailaddress == emailAddress) {
        isDuplicateUser = true;
      }
    });

    if (isDuplicateUser) {
      return;
    }

    // check if user exists in the database
    var data = {
      email: emailAddress
    };
    UserProfileService.getAllUsers(data).then(function (profiles) {
      if (profiles.length === 1) {
        $scope.customerGroup.adminusers.push(profiles[0]);
      } else {
        // display error message to user
        UserProfileService.createUserFromAdmin(data).then(function (profiles) {
          if (profiles.length === 1) {
            $scope.customerGroup.adminusers.push(profiles[0]);
          } else {
            // display error message to user
          }
        });
      }
    });
  };

  $scope.removeUser = function(user) {
    var idx = $scope.customerGroup.adminusers.indexOf(user);
    $scope.customerGroup.adminusers.splice(idx, 1);
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
        $log.debug(error);
        $scope.displayMessage('error', 'Error creating new customer group.');
      }).finally(function() {
        processingCreateCustomerGroup = false;
      });
    }
  };

  var processingSaveCustomerGroup = false;
  function saveCustomerGroup(group) {
    if (!processingSaveCustomerGroup) {
      processingSaveCustomerGroup = true;
      delete group.customerusers;
      CustomerGroupService.updateGroup(group).then(function(groups) {
        $scope.displayMessage('success', 'Successfully saved customer group.');
      }, function(error) {
        $scope.displayMessage('error', 'Error saving customer group.');
      }).finally(function() {
        processingSaveCustomerGroup = false;
      });
    }
  };

  $scope.submitForm = function(group) {
    if ($scope.isNew) {
      createNewCustomerGroup(group);
    } else {
      saveCustomerGroup(group);
    }
  }

}]);
