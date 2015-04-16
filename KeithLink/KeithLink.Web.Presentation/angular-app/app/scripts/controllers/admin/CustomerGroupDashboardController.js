'use strict';

angular.module('bekApp')
  .controller('CustomerGroupDashboardController', ['$scope', '$log', '$stateParams', '$state', 'UserProfileService', 'CustomerService', 'CustomerGroupService', 'BroadcastService', 'CustomerPagingModel',
    function (
      $scope, $log, // angular
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
  
  $scope.clearFilter = function(){    
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
        $log.debug(errorMessage);
        $scope.displayMessage('error', 'An error occurred creating the user: ' + errorMessage);
      }).finally(function() {
        processingCreateUser = false;
      });
    }
  }

  $scope.checkUser = function (checkEmail) {  
    //set email as a parameter
    var params = {
      email: checkEmail
    };
    $scope.checkUserExists = false;
    $scope.canMoveUser = false;
    $scope.cannotMoveUser = false;
    $scope.canAddUser = false;
    $scope.checkEmail = checkEmail;
    
    //check if user exists in the database
    UserProfileService.getAllUsers(params).then(function (profiles) {
      //if the user does exist update userExists flag to true, else keep it as false
      if (profiles.length) {
        $scope.checkUserExists = true; // displays error message

        // check if user is on a customer group
        CustomerGroupService.getGroups({
          from: 0,
          size: 50,
          type: 'user',
          terms: checkEmail
        }).then(function(customerGroups) {
          // if user is already associated with a customer group
          if (customerGroups.totalResults) {
            // check if user already exists on the current customer group
            $scope.userOnCurrentCustomerGroup = false;
            customerGroups.results.forEach(function(customerGroup) {
              if (customerGroup.id === $scope.customerGroup.id) {
                $scope.userOnCurrentCustomerGroup = true;
              }
            });
            if ($scope.userOnCurrentCustomerGroup === false) {
              // sys admin can move the user from one group to another
              if ($scope.canMoveUserToAnotherGroup) {
                $scope.canMoveUser = true;
              } else {
                $scope.cannotMoveUser = true;
              }
            }
          } else {
            // allow owner to add user
            $scope.canAddUser = true; 
          }
        });
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
  };

  resetMessageFields();

  $scope.addCustomerToRecipients = function (customer) {
    var newEntry = {};
    newEntry.displayName = customer.customerName;
    newEntry.id = customer.customerId;
    $scope.customerRecipients.push(newEntry);
  };

  $scope.addUserToRecipients = function (user) {
    var newEntry = {};
    newEntry.displayName = user.firstname + '   ' + user.lastname;
    newEntry.id = user.userid;
    $scope.userRecipients.push(newEntry);
  };

  $scope.removeFromRecipients = function(recipientId, recipientList) {
    recipientList.forEach(function (current, index) {
      if (recipientId === current.id){
        recipientList.splice(index, 1);
      }
    });
  };

  $scope.sendMessage = function (broadcast, customerRecipients, userRecipients) {
    var payload = {
      customers: [],
      users: [],
      message: {
        label: 'Admin Message', // ??
        subject: broadcast.subject,
        body: broadcast.bodyContent,
        mandatory: false // ??
      }
    };

    customerRecipients.forEach(function(customer) {
      payload.customers.push(customer.id);
    });
    userRecipients.forEach(function(user) {
      payload.users.push(user.id);
    });
    
    // $log.debug(payload);
    BroadcastService.broadcastMessage(payload).then(function (success) {
      $scope.displayMessage('success', 'The message was sent successfully.');
      resetMessageFields(); //reset message inputs
    }, function (error) {
      $scope.displayMessage('error', 'There was an error sending the message: ' + error);
    });
  };

  

}]);
