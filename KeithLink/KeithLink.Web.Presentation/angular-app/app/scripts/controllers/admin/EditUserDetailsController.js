'use strict';

angular.module('bekApp')
  .controller('EditUserDetailsController', ['$scope', '$q', '$state', '$stateParams', '$modal', 'UserProfileService', 'userProfile', 'userCustomers',
    function ($scope, $q, $state, $stateParams, $modal, UserProfileService, userProfile, userCustomers) {

  $scope.groupId = $stateParams.groupId;
  
  function checkIfUserExistsOnAnotherGroup() {
    // add check if userCustomers are in a different customer group
    // throw warning to user that assigning different customers will change the customer group
    var userIsOnAnotherCustomerGroup = false;
    userCustomers.forEach(function(customer) {
      if (customer.accountId !== $scope.groupId) {
        userIsOnAnotherCustomerGroup = true;
      }
    });

    if (userIsOnAnotherCustomerGroup) {
      var r = confirm('This user has access to a different customer group than the one selected. Assigning customers will remove the user from the previous customer group. Are you sure you want to continue?');
      if (r === true) {

      } else {
        // redirect to dashboard
        $state.go('menu.admin.customergroupdashboard', { customerGroupId: $stateParams.groupId });
      }
    }
  }

  var email;
  var processProfile = function(newProfile) {
    // rename email <----- NEEDS FIX ON RESPONSE TYPE
    newProfile.email = newProfile.emailaddress;
    delete newProfile.emailaddress;
    email = newProfile.email;

    // rename role <----- NEEDS FIX ON RESPONSE TYPE
    newProfile.role = newProfile.rolename;
    delete newProfile.rolename;

    $scope.profile = newProfile;
    $scope.profile.customers = userCustomers;
  };


  // TODO: get available roles <----NEEDS ENDPOINT
  $scope.roles =  [{ "value": 'owner', "text": "owner" }, { "value": 'accounting', "text": "accounting" },{ "value": 'approver', "text": "buyer" }, { "value": 'buyer', "text": "shopper" }, { "value": 'guest', "text": "guest" }];

  processProfile(userProfile);
  checkIfUserExistsOnAnotherGroup();

  /**********
  FORM EVENTS
  **********/

  var processingSaveProfile = false;
  $scope.updateProfile = function (profile) {
    var deferred = $q.defer();
    if (!processingSaveProfile) {
      processingSaveProfile = true;
      UserProfileService.updateUserProfile(profile).then(function(newProfile){
        // update currently logged in user profile
        if ($scope.$parent.$parent.userProfile.userid === newProfile.userid) {
          $scope.$parent.$parent.userProfile = newProfile;
        }
        $scope.editUserForm.$setPristine();
        $scope.displayMessage('success', 'The user was successfully updated.');
        deferred.resolve();
      }, function(error){
        $scope.displayMessage('error', 'An error occurred: ' + error);
        deferred.reject();
      }).finally(function() {
        processingSaveProfile = false;
      });
    } else {
      deferred.reject();
    }
    return deferred.promise;
  };

  $scope.deleteProfile = function (profile) {
    var customerGroupId = $stateParams.groupId;
    UserProfileService.removeUserFromCustomerGroup(profile.userid, customerGroupId).then(function(newProfile){
      $scope.editUserForm.$setPristine();
      $scope.displayMessage('success', 'The user was successfully removed.');
      $state.go('menu.admin.customergroupdashboard', { customerGroupId: customerGroupId });
    }, function(error){
      $scope.displayMessage('error', 'An error occurred: ' + error);
    });
  };

  $scope.changeUserAccess = function(isGrantingAccess, program) {
    UserProfileService.changeProgramAccess(email, program, isGrantingAccess);
  };


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
          return $scope.groupId;
        },
        selectedCustomers: function() {
          return $scope.profile.customers;
        }
      }
    });

    modalInstance.result.then(function(selectedCustomers) {
      
      // save new customers
      var profile = angular.copy($scope.profile);
      profile.customers = $scope.profile.customers.concat(selectedCustomers);
      $scope.updateProfile(profile).then(function() {
        $scope.profile = profile;
      });
    });
  };

  $scope.selectAllCustomers = function(allSelected) {
    $scope.profile.customers.forEach(function(availableCustomer) {
      availableCustomer.selected = allSelected;
    });
  };

  $scope.removeSelectedCustomers = function() {
    $scope.editUserForm.$setDirty();

    var assignedCustomers = [];
    $scope.profile.customers.forEach(function(customer) {
      if (customer.selected !== true) {
        assignedCustomers.push(customer);
      }
    });

    // console.log(assignedCustomers);
    $scope.profile.customers = assignedCustomers;
  };
  
}]);
