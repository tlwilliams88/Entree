'use strict';

angular.module('bekApp')
  .controller('CustomerGroupDashboardController', ['$scope', '$stateParams', '$state', 'UserProfileService', 'CustomerService', 'CustomerGroupService', 'BroadcastService', 'CustomerPagingModel',
    function (
      $scope, // angular
      $stateParams, $state, // ui router
      UserProfileService, CustomerService, CustomerGroupService, BroadcastService, CustomerPagingModel // custom bek services
    ) {

  function getCustomerGroupDetails(customerGroupId) {
    CustomerGroupService.getGroupDetails(customerGroupId).then(function(customerGroup) {
      $scope.loadingUsers = false;
      $scope.customerGroup = customerGroup;
    }, function() {
      $scope.displayMessage('error', 'An error occured loading the customer group dashboard page (ID: ' + customerGroupId + ').');
      $scope.goToAdminLandingPage();
    });
  }

  $scope.loadingUsers = true;
  
  // set correct user details link based on role
  $scope.userDetailState = 'menu.admin.user.view';
  if ($scope.canEditUsers) { // inherited from MenuController
    $scope.userDetailState = 'menu.admin.user.edit';
  }

  // get Customer Group Details
  if ($stateParams.customerGroupId) {
    // internal bek admins
    getCustomerGroupDetails($stateParams.customerGroupId);
  } else {
    // get group for external admin using the customer group associated with their user id
    CustomerGroupService.getGroupByUser($scope.userProfile.userid).then(function(customerGroup) {
      getCustomerGroupDetails(customerGroup.id);
    });
  }

  /**********
  CUSTOMERS
  **********/

  function setCustomers(data) {
    $scope.customers = data.results;
    $scope.totalCustomers = data.totalResults;
  }
  function appendCustomers(data) {
    $scope.customers = $scope.customers.concat(data.results);
  }
  function startLoading() {
    $scope.loadingCustomers = true;
  }
  function stopLoading() {
    $scope.loadingCustomers = false;
  }

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
  if ($stateParams.customerGroupId) {
    customerPagingModel.accountId = $stateParams.customerGroupId;
  }

  customerPagingModel.loadCustomers();

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

  /**********
  USERS
  **********/

  var processingCreateUser;
  function createUser(email) {
    var newProfile = {
      email: email,
      branchId: ''
    };

    if (!processingCreateUser) {
      UserProfileService.createUserFromAdmin(newProfile).then(function (profile) {
        processingCreateUser = true;
        // TODO: user must immediately assign customers to the new user or else that user will be lost in limbo
        //redirects to user profile page
        $state.go('menu.admin.user.edit', { groupId: $scope.customerGroup.id, email: email });
      }, function (errorMessage) {
        console.log(errorMessage);
        $scope.displayMessage('error', 'An error occurred creating the user: ' + errorMessage);
      }).finally(function() {
        processingCreateUser = false;
      });
    }
  }
  
  $scope.userExists = false;
  $scope.checkUser = function (checkEmail) {  
    //set email as a parameter
    var params = {
      email: checkEmail
    };
    //check if user exists in the database
    $scope.userExists = false;
    UserProfileService.getAllUsers(params).then(function (profile) {
      //if the user does exist update userExists flag to true, else keep it as false
      if (profile.length) {
        $scope.userExists = true; //displays error message
      } else {
        //make user profile then redirect to profile page
        createUser(checkEmail);
      }
    }, function (errorMessage) {
      $scope.displayMessage('error', 'An error occurred checking if the user exists: ' + errorMessage);
    });
  };

  /**********
  MESSAGING
  **********/

  var resetMessageFields = function() {
    $scope.broadcast = {};
    $scope.customerRecipients = [];
    $scope.userRecipients = [];
  }

  resetMessageFields();

  $scope.addCustomerToRecipients = function (customer) {
    var newEntry = {};
    newEntry.displayName = customer.customerName;
    newEntry.id = customer.customerId;
    $scope.customerRecipients.push(newEntry);
  };

  $scope.addUserToRecipients = function (user) {
    var newEntry = {};
    newEntry.displayName = user.firstname + " " + user.lastname;
    newEntry.id = user.userid;
    $scope.userRecipients.push(newEntry);
  };

  $scope.removeFromRecipients = function(recipientId, recipientList) {
    recipientList.forEach(function (current, index) {
      if (recipientId === current.id){
        recipientList.splice(index, 1);
      }
    })
  };

  $scope.sendMessage = function (broadcast, customerRecipients, userRecipients) {
    var payload = {
      customers: customerRecipients,
      users: userRecipients,
      message: {
        label: 'Admin Message', // ??
        subject: broadcast.subject,
        body: broadcast.bodyContent,
        mandatory: false // ??
      }
    };
    
    console.log(payload);
    BroadcastService.broadcastMessage(payload).then(function (success) {
      $scope.displayMessage('success', 'The message was sent successfully.');
      resetMessageFields(); //reset message inputs
    }, function (error) {
      $scope.displayMessage('error', 'There was an error sending the message: ' + error);
    });
  };

  

}]);
