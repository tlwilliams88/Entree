'use strict';

angular.module('bekApp')
  .controller('EditUserDetailsController', ['$scope', '$state', '$stateParams', 'UserProfileService', 'userProfile', 'userCustomers', 'CustomerPagingModel',
    function ($scope, $state, $stateParams, UserProfileService, userProfile, userCustomers, CustomerPagingModel) {

  var processProfile = function(newProfile) {
    // rename email <----- NEEDS FIX ON RESPONSE TYPE
    newProfile.email = newProfile.emailaddress;
    delete newProfile.emailaddress;

    // rename role <----- NEEDS FIX ON RESPONSE TYPE
    newProfile.role = newProfile.rolename;
    delete newProfile.rolename;

    $scope.profile = newProfile;
    $scope.profile.customers = userCustomers;
  };

  function findSelectedCustomers(customers) {
    // check if customer is selected
    customers.forEach(function(customer) {
      $scope.profile.customers.forEach(function(profileCustomer) {
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

  $scope.groupId = $stateParams.groupId;
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
  customerPagingModel.accountId = $scope.groupId; 

  // TODO: get available roles <----NEEDS ENDPOINT
  $scope.roles = ['owner', 'accounting', 'approver', 'buyer', 'guest'];

  processProfile(userProfile);
  customerPagingModel.loadCustomers(),

  /**********
  FORM EVENTS
  **********/

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
    UserProfileService.updateUserProfile($scope.profile).then(function(newProfile){
      $scope.displayMessage('success', 'The user was successfully updated.');
    }, function(error){
      $scope.displayMessage('error', 'An error occurred: ' + error);
    });
  };

  $scope.deleteProfile = function (profile) {
    var customerGroupId = $stateParams.groupId;
    UserProfileService.removeUserFromCustomerGroup(profile.userid, customerGroupId).then(function(newProfile){
      $scope.displayMessage('success', 'The user was successfully removed.');
      $state.go('menu.admin.customergroupdashboard', { customerGroupId: customerGroupId });
    }, function(error){
      $scope.displayMessage('error', 'An error occurred: ' + error);
    });
  };

  /**********
  CUSTOMERS
  **********/
  
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
    $scope.profile.customers.push(customer);
    customer.selected = true;
  };

  $scope.unselectCustomer = function(customer) {
    var idx = $scope.profile.customers.indexOf(customer);
    $scope.profile.customers.splice(idx, 1);
    $scope.customers.forEach(function(availableCustomer) {
      if (customer.customerNumber === availableCustomer.customerNumber) {
        availableCustomer.selected = false;
      }
    });
    customer.selected = false;
  };

}]);
