'use strict';

angular.module('bekApp')
  .controller('CustomerGroupDashboardController', ['$scope', '$stateParams', '$state', 'UserProfileService', 'CustomerService', 'CustomerGroupService', 'BroadcastService',
    function (
      $scope, // angular
      $stateParams, $state, // ui router
      UserProfileService, CustomerService, CustomerGroupService, BroadcastService // custom bek services
    ) {

  function getUsers(customerGroup) {
   if (customerGroup) {
      $scope.customerGroupId = customerGroup.id;
      $scope.groupName = customerGroup.name;
      
      UserProfileService.getUsersForGroup(customerGroup.id).then(function(data) {
        $scope.loadingUsers = false;
        $scope.adminUsers = data.accountUsers;
        $scope.customerUsers = data.customerUsers;
      });
    } else {
      $scope.displayMessage('error', 'An error occured loading the customer group dashboard page.');
      $scope.goToAdminLandingPage();
    }
  }

  function init() {
    loadCustomers(customersConfig).then(setCustomers);

    $scope.loadingUsers = true;

    if ($stateParams.customerGroupId) {
      // internal bek admins
      CustomerGroupService.getGroupDetails($stateParams.customerGroupId).then(
        getUsers, 
        function(error) {
          // request failed, invalid customer group id
          $scope.displayMessage('error', 'An error occured loading the page with Customer Group ID # ' + $stateParams.customerGroupId);
          $scope.goToAdminLandingPage(); // inherited from MenuController
      });
    
    } else {
      // get group for external admin using the customer group associated with their user id
      CustomerGroupService.getGroupByUser($scope.userProfile.userid).then(getUsers);
    }

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
    sortOrder: 'asc',
    customerGroupId: ''
  };

  if ($stateParams.customerGroupId) {
    customersConfig.customerGroupId = $stateParams.customerGroupId;
  }

  function loadCustomers(customersConfig) {
    $scope.loadingCustomers = true;
    return CustomerService.getCustomers(
      customersConfig.term,
       customersConfig.size,
       customersConfig.from,
       customersConfig.sortField,
       customersConfig.sortOrder,
       customersConfig.customerGroupId
    ).then(function(data) {
      $scope.loadingCustomers = false;
      $scope.totalCustomers = data.totalResults;
      return data.results;
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

  /**********
  USERS
  **********/
  $scope.userExists = false;
  $scope.checkUser = function (checkEmail) {  
    //set email as a parameter
    var data = {
      email: checkEmail
    };
    //check if user exists in the database
    UserProfileService.getAllUsers(data).then(
      function (profile) {
        //if the user does exist update userExists flag to true, else keep it as false
        if (profile.length) {
          //displays error message
          $scope.userExists = true;
          //$state.go('menu.admin.edituser', {email : checkEmail});
        } else {
          //make user profile then redirect to profile page
          var newProfile = {};
          newProfile.email = checkEmail;
          newProfile.branchId = "";

          //sends new User Profile to db and receives newly generated profile object
          UserProfileService.createUserFromAdmin(newProfile).then(
            function (profile) {
              //redirects to user profile page
              $state.go('menu.admin.edituser', {groupId: $scope.customerGroupId, email: checkEmail});
            }, function (errorMessage) {
              console.log(errorMessage);
              $scope.displayMessage('error', 'An error occurred creating the user: ' + errorMessage);
            });
        }
      }, function (errorMessage) {
        $scope.displayMessage('error', 'An error occurred checking if the user exists: ' + errorMessage);
      });
  };

  /*---Broadcast Message Functions---*/

  $scope.recipients = [];
  $scope.broadcast = {};

  $scope.addCustomerToRecipients = function (customer) {
    var newEntry = {};
    newEntry.displayName = customer.customerName;
    newEntry.id = customer.customerId;
    newEntry.type = 'Customer';
    $scope.recipients.push(newEntry);
  };

  $scope.addUserToRecipients = function (user) {
    var newEntry = {};
    newEntry.displayName = user.firstname + " " + user.lastname;
    newEntry.id = user.userid;
    newEntry.type = 'User';
    $scope.recipients.push(newEntry);
  };

  $scope.removeFromRecipients = function(recipient) {
    $scope.recipients.forEach(function (current, index) {
      if (recipient.id === current.id){
        $scope.recipients.splice(index, 1);
      }
    })
  };

  $scope.sendMessage = function () {
    var payload = {};
    payload.customers = [];
    payload.users = [];

    //construct payload
    $scope.recipients.forEach(function (recipient) {
      if (recipient.type === 'customer'){
        payload.customers.push(recipient.id);
      } else {
        payload.users.push(recipient.id);
      }
    });
    payload.message = {};
    payload.message.label = 'Admin Message'; //ask about this
    payload.message.subject = $scope.broadcast.subject;
    payload.message.body = $scope.broadcast.bodyContent;
    payload.message.mandatory = false; //ask about this option

    console.log(payload);
    BroadcastService.broadcastMessage(payload).then(function (success) {
      $scope.displayMessage('success', 'The message was sent successfully.');
      clearFields(); //reset message inputs
    }, function (error) {
      $scope.displayMessage('error', 'There was an error sending the message: ' + error);
    });
  };

  var clearFields = function(){
    $scope.recipients = [];
    $scope.broadcast = {};
  }

  init();

}]);
