'use strict';

angular.module('bekApp')
  .controller('EditUserDetailsController', ['$scope', 'UserProfileService', 'userProfile', 'AccountService', 'CustomerService',
    function ($scope, UserProfileService, userProfile, AccountService, CustomerService) {

  /*---convenience functions---*/
  var processProfile = function(newProfile){
    // rename email <----- NEEDS FIX ON RESPONSE TYPE
    newProfile.email = newProfile.emailaddress;
    delete newProfile.emailaddress;

    // rename role <----- NEEDS FIX ON RESPONSE TYPE
    newProfile.role = newProfile.rolename;
    delete newProfile.rolename;

    // rename customers <----- NEEDS FIX ON RESPONSE TYPE
    newProfile.customers = newProfile.user_customers;
    delete newProfile.user_customers;

    if (!newProfile.customers) {
      newProfile.customers = [];
    }
    $scope.profile = newProfile;
  };

  /*---Init---*/
  function init() {
    //$scope.roles = RoleService.getRoles(); //get available roles <----NEEDS ENDPOINT
    $scope.roles = ['owner', 'accounting', 'approver', 'buyer', 'guest'];

    processProfile(userProfile);

    loadAvailableCustomers(customersConfig).then(setCustomers);
  }

  /*---edit profile---*/
  $scope.updateProfile = function () {
    //attaches only selected customers to profile object before it is pushed to the database
    var selectedCustomers = [];
    $scope.customers.forEach(function(customer){
      if(customer.selected){
        selectedCustomers.push(customer);
      }
    });

    $scope.profile.customers = selectedCustomers;

    //pushes profile object to database
    UserProfileService.updateProfile($scope.profile).then(function(newProfile){
      $scope.displayMessage('success', 'The user was successfully updated.');
      //processProfile(newProfile); // <-- UNCOMMENT WHENEVER DATA SENT BACK IS FRESH
    }, function(error){
      $scope.displayMessage('error', 'An error occurred: ' + error);
    });
  };

  // TODO: better way to do this?
  $scope.deleteProfile = function (profile) {
    //wipe customers out of user profile and set profile to lowest permission role
    profile.role = 'guest';
    profile.customers = [];

    //push freshly wiped profile to database
    UserProfileService.updateProfile(profile).then(function(newProfile){
      //refreshes page with newest data
      processProfile(newProfile);
      $scope.displayMessage('success', 'The user was successfully deleted.');
    }, function(error){
      $scope.displayMessage('error', 'An error occurred: ' + error);
    });
  };

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

  function loadAvailableCustomers(customersConfig) {
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
        $scope.profile.customers.forEach(function(selectedCustomer) {
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
    loadAvailableCustomers(customersConfig).then(setCustomers);
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
    
    loadAvailableCustomers(customersConfig).then(setCustomers);
  };

  $scope.infiniteScrollLoadMore = function() {
    if (($scope.customers && $scope.customers.length >= $scope.totalCustomers) || $scope.loadingCustomers) {
      return;
    }
    customersConfig.from += customersConfig.size;
    loadAvailableCustomers(customersConfig).then(appendCustomers);
  };

  $scope.selectCustomer = function(customer) {
    $scope.profile.customers.push(customer);
    customer.selected = true;
  };

  $scope.unselectCustomer = function(customer) {
    var idx = $scope.profile.customers.indexOf(customer);
    $scope.profile.customers.splice(idx, 1);
    // TODO: loop through customers and change selected value
    customer.selected = false;
  };

  init();
}]);
