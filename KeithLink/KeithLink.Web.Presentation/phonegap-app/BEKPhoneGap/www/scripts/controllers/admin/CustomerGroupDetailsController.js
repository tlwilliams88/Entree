'use strict';

angular.module('bekApp')
  .controller('CustomerGroupDetailsController', ['$scope', '$state', '$stateParams', 'originalCustomerGroup', 'CustomerGroupService', 'CustomerService', 'UserProfileService',
    function ($scope, $state, $stateParams, originalCustomerGroup, CustomerGroupService, CustomerService, UserProfileService) {
    
  function init() {
    if ($stateParams.groupId === 'new') {
      $scope.originalCustomerGroup = {
        customers: [],
        adminusers: []
      };
      $scope.customerGroup = angular.copy($scope.originalCustomerGroup);
      $scope.isNew = true;
    } else {
      $scope.originalCustomerGroup = originalCustomerGroup;
      $scope.customerGroup = angular.copy($scope.originalCustomerGroup);
      $scope.isNew = false;
    }
    
    loadCustomers(customersConfig).then(setCustomers);
  }

  /**********
  CUSTOMERS
  **********/
  $scope.customersSortAsc = true;
  $scope.customersSortField = 'customerName';
  var customersConfig = {
    term: '',
    size: 30,
    from: 0,
    sortField: $scope.customersSortField,
    sortOrder: 'asc'
  };

  function loadCustomers(customersConfig) {
    $scope.loadingCustomers = true;
    return CustomerService.getCustomers(
      customersConfig.term,
       customersConfig.size,
       customersConfig.from,
       customersConfig.sortField,
       customersConfig.sortOrder
    ).then(function(data) {
      $scope.loadingCustomers = false;
      var customers = data.results;
      $scope.totalCustomers = data.totalResults;

      // check if customer is selected
      customers.forEach(function(customer) {
        $scope.customerGroup.customers.forEach(function(selectedCustomer) {
          if (customer.customerId === selectedCustomer.customerId) {
            customer.selected = true;
          }
        });

        if (!customer.selected) {
          customer.selected = false;
        }
      });

      return customers;
    });
  }

  function setCustomers(customers) {
    $scope.customers = customers;
  }
  function appendCustomers(customers) {
    $scope.customers = $scope.customers.concat(customers);
  }

  $scope.searchCustomers = function (searchTerm) {
    customersConfig.from = 0;
    customersConfig.term = searchTerm;
    loadCustomers(customersConfig).then(setCustomers);
  };

  $scope.sortCustomers = function(field, order) {
    customersConfig.from = 0;
    customersConfig.size = 30;
    customersConfig.sortField = field;
    $scope.customersSortField = field;

    $scope.customersSortAsc = order;
    if (order) {
      customersConfig.sortOrder = 'asc';
    } else {
      customersConfig.sortOrder = 'desc';
    }
    
    loadCustomers(customersConfig).then(setCustomers);
  };

  $scope.infiniteScrollLoadMore = function() {
    if (($scope.customers && $scope.customers.length >= $scope.totalCustomers) || $scope.loadingCustomers) {
      return;
    }
    customersConfig.from += customersConfig.size;
    loadCustomers(customersConfig).then(appendCustomers);
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
        $state.go('menu.admin.editcustomergroup', { groupId: newGroup.id });
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

  init();

}]);
