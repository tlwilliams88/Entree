'use strict';

angular.module('bekApp')
  .controller('AdminAccountDetailsController', ['$scope', '$stateParams', 'originalAccount', 'AccountService', 'CustomerService', 'UserProfileService',
    function ($scope, $stateParams, originalAccount, AccountService, CustomerService, UserProfileService) {
    
  function init() {
    if ($stateParams.accountId === 'new') {
      $scope.originalAccount = {
        customers: [],
        adminusers: []
      };
      $scope.account = angular.copy($scope.originalAccount);
      $scope.isNew = true;
    } else {
      $scope.originalAccount = originalAccount;
      $scope.account = angular.copy($scope.originalAccount);
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
        $scope.account.customers.forEach(function(selectedCustomer) {
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
    $scope.account.customers.push(customer);
    customer.selected = true;
  };

  $scope.unselectCustomer = function(customer) {
    var idx = $scope.account.customers.indexOf(customer);
    $scope.account.customers.splice(idx, 1);
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
    $scope.account.adminusers.forEach(function(user) {
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
        $scope.account.adminusers.push(profiles[0]);
      } else {
        // display error message to user
      }
    });
  };

  $scope.removeUser = function(user) {
    var idx = $scope.account.adminusers.indexOf(user);
    $scope.account.adminusers.splice(idx, 1);
  };

  /***********
  FORM EVENTS
  ***********/
  function createNewAccount(account) {
    AccountService.createAccount(account).then(function(newAccount) {
      $scope.displayMessage('success', 'Successfully created a new account.');
      $state.go('menu.admin.accountdetails', { accountId: newAccount.id });
    }, function(error) {
      $log.debug(error);
      $scope.displayMessage('error', 'Error creating new account.');
    });
  };

  function saveAccount(account) {
    delete account.customerusers;
    AccountService.updateAccount(account).then(function(accounts) {
      console.log(accounts);
    });
  };

  $scope.submitForm = function(account) {
    if ($scope.isNew) {
      createNewAccount(account);
    } else {
      saveAccount(account);
    }
  }

  $scope.cancelChanges = function() {
    $scope.account = angular.copy($scope.originalAccount);
  };

  init();

}]);
