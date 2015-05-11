'use strict';

angular.module('bekApp').controller('CustomerGroupDetailsController', ['$scope', '$state', '$stateParams', '$modal', '$filter', 'originalCustomerGroup', 'CustomerGroupService', 'UserProfileService', function($scope, $state, $stateParams, $modal, $filter, originalCustomerGroup, CustomerGroupService, UserProfileService) {
  
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

  $scope.openCustomerAssignmentModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/customerassignmentmodal.html',
      controller: 'CustomerAssignmentModalController',
      size: 'lg',
      resolve: {
        customerGroupId: function() {
          return null;
        },
        selectedCustomers: function() {
          return $scope.customerGroup.customers;
        }
      }
    });

    modalInstance.result.then(function(selectedCustomers) {

      $scope.customerGroup.customers = $scope.customerGroup.customers.concat(selectedCustomers);
      saveCustomerGroup($scope.customerGroup);

    });
  };

  $scope.selectAllCustomers = function(allSelected) {
    $scope.customerGroup.customers.forEach(function(availableCustomer) {
      availableCustomer.selected = allSelected;
    });
  };

  $scope.removeSelectedCustomers = function() {
    $scope.customerGroupDetailsForm.$setDirty();

    var assignedCustomers = [];
    $scope.customerGroup.customers.forEach(function(customer) {
      if (customer.selected !== true) {
        assignedCustomers.push(customer);
      }
    });

    // console.log(assignedCustomers);
    $scope.customerGroup.customers = assignedCustomers;
  };

  /**********
  USERS
  **********/

  $scope.addUserError = '';
  $scope.checkUserAndAddAdmin = function(emailAddress) {
    if (emailAddress) {
      $scope.dirty = true;
    }
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
    UserProfileService.getAllUsers(data).then(function(profiles) {
      if (profiles.length === 1) {
        $scope.customerGroup.adminusers.push(profiles[0]);
        $scope.submitForm($scope.customerGroup);
      } else {
        // display error message to user
        UserProfileService.createUserFromAdmin(data).then(function(profiles) {
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

  $scope.goBack = function() {
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
        $state.go('menu.admin.customergroupdetails', {
          groupId: newGroup.id
        });
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
