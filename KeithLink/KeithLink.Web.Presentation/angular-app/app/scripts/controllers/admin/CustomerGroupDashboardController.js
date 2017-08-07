'use strict';

angular.module('bekApp')
  .controller('CustomerGroupDashboardController', ['$scope', '$q', '$log', '$stateParams', '$state', '$modal', '$filter', 'toaster', 'UserProfileService', 'CustomerGroupService', 'MessagingService', 'BranchService',
    function (
      $scope, $q, $log, // angular
      $stateParams, $state, $modal, $filter, toaster,// ui router
      UserProfileService, CustomerGroupService, MessagingService, BranchService // custom bek services
    ) {

  function getCustomerGroupDetails(customerGroupId) {
    CustomerGroupService.getGroupDetails(customerGroupId).then(function(customerGroup) {
      $scope.loadingResults = false;
      $scope.customerGroup = customerGroup;
      $scope.nonAdminCustUsers = [];
      customerGroup.customerusers.forEach(function(customerUser){
        var matchFound = false;
        customerGroup.adminusers.forEach(function(adminUser){
          if(adminUser.emailaddress === customerUser.emailaddress){
            matchFound = true;
          }
        });
        if(!matchFound){
          $scope.nonAdminCustUsers.push(customerUser);
        }
      });
    }, function() {
      $scope.displayMessage('error', 'An error occured loading the customer group dashboard page (ID: ' + customerGroupId + ').');
      $scope.goToAdminLandingPage();
    });
  }

  $scope.loadingResults = true;

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

  /**
   * CUSTOMER GROUP
   */

  var processingSaveCustomerGroup = false;
  function saveCustomerGroup(group , showMessage) {
    var duplicateName = false;
    var deferred = $q.defer();
    if (!processingSaveCustomerGroup) {
      processingSaveCustomerGroup = true;
      var custUsers = group.customerusers;
      delete group.customerusers;
      CustomerGroupService.updateGroup(group).then(function() {
        group.customerusers = custUsers;
        if(showMessage){
          $scope.displayMessage('success', 'Successfully saved customer group.');
        }
        deferred.resolve();
      }, function(error) {
        group.customerusers = custUsers;
        var message = error ? error : 'Error updating customer group.';
        $scope.displayMessage('error', message);
        deferred.reject();
      }).finally(function() {
        processingSaveCustomerGroup = false;
      });
    } else {
      // event still processing
      deferred.reject();
    }
    return deferred.promise;
  }

  $scope.deleteGroup = function(id) {
    CustomerGroupService.deleteGroup(id).then(function() {
      $state.go('menu.admin.customergroup');
      $scope.displayMessage('success', 'Successfully deleted customer group.');
    }, function() {
      $scope.displayMessage('error', 'Error deleting customer group.');
    });
  };

  /**
   * CUSTOMERS
   */

  $scope.customersSortDesc = false;
  $scope.customersSortField = 'customerName';

  $scope.sortCustomers = function(field, sortDescending) {
    $scope.customersSortDesc = $scope.customersSortField === field ? !sortDescending : false;
    $scope.customersSortField = field;
  };

  $scope.openCustomerAssignmentModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/customerassignmentmodal.html',
      controller: 'CustomerAssignmentModalController',
      size: 'lg',
      resolve: {
        customerGroupId: function() {
          return null; // used for edit user page to limit which customers can be assigned
        },
        selectedCustomers: function() {
          return $scope.customerGroup.customers;
        }
      }
    });

    modalInstance.result.then(function(selectedCustomers) {
      // save new customers
      var group = angular.copy($scope.customerGroup);
      group.customers = $scope.customerGroup.customers;
      selectedCustomers.forEach(function(customer){
       if(customer.accountId){
          toaster.pop('error', null, 'Could not complete the request. Customer '+customer.displayname+' is already a member of a Customer Group.');
        }
        else{
          group.customers.push(customer);
        }
      });
      saveCustomerGroup(group , false).then(function() {
        $scope.customerGroup = group;
      });
    });
  };

  $scope.selectAllCustomers = function(allSelected) {
    $scope.customerGroup.customers.forEach(function(availableCustomer) {
      availableCustomer.selected = allSelected;
    });
  };

  $scope.removeSelectedCustomers = function() {
    var assignedCustomers = [];
    $scope.customerGroup.customers.forEach(function(customer) {
      if (customer.selected !== true) {
        assignedCustomers.push(customer);
      }
    });

    $scope.customerGroup.customers = assignedCustomers;

    saveCustomerGroup($scope.customerGroup , true);
  };

  /**
   * USERS
   */

  var processingCreateUser = false;
  function createUser(email) {
    var deferred = $q.defer();

    var newProfile = {
      email: email
      // , branchId: ''
    };

    if (!processingCreateUser) {
      processingCreateUser = true;
      UserProfileService.createUserFromAdmin(newProfile).then(function (profiles) {
        deferred.resolve(profiles[0]);
      }, function (errorMessage) {
        $log.debug(errorMessage);
        deferred.reject(errorMessage);
        $scope.displayMessage('error', 'An error occurred creating the user: ' + errorMessage);
      }).finally(function() {
        processingCreateUser = false;
      });
    } else {
      deferred.reject();
    }
    return deferred.promise;
  }

  $scope.addCustomerUser = function (checkEmail) {
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

          // check if user is already associated with a customer group
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
        // make user profile then redirect to profile page
        createUser(checkEmail).then(function(profile) {
          $state.go('menu.admin.user.edit', { groupId: $scope.customerGroup.id, email: profile.emailaddress });
        });
      }
    }, function (errorMessage) {
      $scope.displayMessage('error', 'An error occurred checking if the user exists: ' + errorMessage);
    });
  };

  $scope.startRenamingGroup = function(group) {
      $scope.currentGroupName = group.name;
      $scope.isRenaming = true;
    };

  $scope.cancelRenamingGroup = function(){
      $scope.isRenaming = false;
    };

  $scope.updateOldGroup = function(group, name){
    var duplicateName = false;
    // getAllGroups for user
    CustomerGroupService.getGroups({
          from: 0,
          size: 50,
          sort: [{order: 'asc', field: 'name'}]
        }).then(function(customerGroups) {

    if (customerGroups) {

      // Cycle through each group and compare group name to new name
      customerGroups.results.forEach(function(customerGroup) {
          if(name === customerGroup.name){
            duplicateName = (customerGroup.id === group.id) ? false : true;
            $scope.isRenaming = duplicateName;
          }
        });

      // If a duplicate is identified throw an error message immediately
      // otherwise continue through groups array and save group
          if(duplicateName){
            $scope.currentGroupName = name;
            document.getElementById('cartName').focus();
            toaster.pop('error', 'Error Saving Group -- Cannot have two groups with the same name. Please try renaming this group once more.');
          }else {
            $scope.isRenaming = false;
            group.name = name;
            saveCustomerGroup(group, true);
          }
      }
    });
  };

   $scope.addExistingUserWithNoGroup = function (profile) {
    $scope.customerGroup.adminusers.push(profile);
    saveCustomerGroup($scope.customerGroup , true).then(function(promise){
      $state.go('menu.admin.user.edit',{email: $scope.checkEmail, groupId: $scope.customerGroup.id});
    });

   };

    $scope.createNewAdminUser = function(){
      createUser($scope.checkEmail).then(function(profile) {
         $scope.addExistingUserWithNoGroup(profile);
      });
    };

    $scope.deleteOldProfile = function (profile,groupid) {
      UserProfileService.removeUserFromCustomerGroup(profile.userid, groupid).then(function(newProfile){
         var data = {
        email: $scope.checkEmail
      };
      UserProfileService.getAllUsers(data).then(function(profiles) {
         $scope.addExistingUserWithNoGroup(profiles[0]);
       });
      }, function(error){
        $scope.displayMessage('error', 'An error occurred: ' + error);
      });
     };

     $scope.addAdminUser = function(emailAddress) {
      // check if user exists in the database
        $scope.adminUserOnCurrentCustomerGroup = null;
        $scope.canAddAdminUser = null;
        $scope.checkAdminUserExists = null;
        $scope.cannotMoveAdminUser =null;
        $scope.canMoveAdminUser = null;
        var data = {
          email: emailAddress
        };
        $scope.checkEmail = emailAddress;
        UserProfileService.getAllUsers(data).then(function(profiles) {
                //if the user does exist update userExists flag to true, else keep it as false
          if (profiles.length) {
            $scope.existingProfile = profiles[0];
            $scope.checkAdminUserExists = true; // displays error message

            // check if user is on a customer group
            CustomerGroupService.getGroups({
              from: 0,
              size: 50,
              type: 'user',
              terms: emailAddress
            }).then(function(customerGroups) {

              // check if user is already associated with a customer group
              if (customerGroups.totalResults) {
                // check if user already exists on the current customer group
                $scope.adminUserOnCurrentCustomerGroup = false;
                customerGroups.results.forEach(function(customerGroup) {
                  if (customerGroup.id === $scope.customerGroup.id) {
                    $scope.adminUserOnCurrentCustomerGroup = true;
                  }
                });
                if ($scope.adminUserOnCurrentCustomerGroup === false) {
                  // sys admin can move the user from one group to another
                  if ($scope.canMoveUserToAnotherGroup) {
                    $scope.origCustGroup = customerGroups.results[0];
                    $scope.canMoveAdminUser = true;
                  } else {
                    $scope.cannotMoveAdminUser = true;
                  }
                }
              } else {
                // allow owner to add user
                $scope.canAddAdminUser = true;
              }
            });
          } else {
              $scope.createNewAdminUser();
          }
          }, function (errorMessage) {
          $scope.displayMessage('error', 'An error occurred checking if the user exists: ' + errorMessage);
        });
      };

  /**
   * MESSAGING
   */


  $scope.resetMessageFields = function() {
    $scope.broadcast = {};
    $scope.selectedBranch = {};
    $scope.customerRecipients = [];
    $scope.userRecipients = [];
    $scope.branchRecipients = [];
    $scope.allUsersSelected = false;
    $scope.isMandatory = false;
    $scope.isSystemUpdate = false;
    $scope.availableBranches = [
      {name: 'All Users', id: 'Entree', selected: false}
    ];
    if(!$scope.branches) {
      BranchService.getBranches().then(function(resp) {
        $scope.branches = resp;
        
        $scope.branches.forEach(function(branch){
          $scope.availableBranches.push({name: branch.name, id: branch.id, selected: false});
        });
      })
  } else {
      $scope.branches.forEach(function(branch){
        $scope.availableBranches.push({name: branch.name, id: branch.id, selected: false});
      });
  }


  };

  $scope.resetMessageFields();

  $scope.selectAllUsers = function(state) {
      var allUsersBranch = $filter('filter')($scope.availableBranches, {name: 'All Users'});
      
      if(state == true) {
          $scope.selectBranch(allUsersBranch[0]);
      } else {
          $scope.deselectBranch(allUsersBranch[0].name);
          $scope.removeFromRecipients(allUsersBranch[0].id, $scope.branchRecipients);
      }
      
  };
  
   $scope.selectBranch = function(selected){
    if(!$scope.allUsersSelected){
      $scope.availableBranches.forEach(function(branch){
        if(branch.name === selected.name){
          branch.selected = true;
          $scope.addBranchToRecipients(branch);
        }
      });
    }
  };

  $scope.addCustomerToRecipients = function (customer) {
    if(!$scope.isMandatory && $filter('filter')($scope.customerRecipients, {id: customer.customerId}).length === 0){
    var newEntry = {};
    newEntry.displayName = customer.customerName;
    newEntry.id = customer.customerId;

      $scope.customerRecipients.push(newEntry);
    }
  };

  $scope.addUserToRecipients = function (user) {
    if(!$scope.isMandatory && $filter('filter')($scope.userRecipients, {id: user.userid}).length === 0){
    var newEntry = {};
      newEntry.displayName = user.firstname + '   ' + user.lastname;
      newEntry.id = user.userid;

        $scope.userRecipients.push(newEntry);
      }
  };

  $scope.addBranchToRecipients = function (addedBranch) {
    var newEntry = {};
    newEntry.displayName = addedBranch.name;
    newEntry.id = addedBranch.id;

    if(addedBranch.name === 'All Users'){
      $scope.branchRecipients = [];
      $scope.allUsersSelected = true;
      $scope.availableBranches.forEach(function(branch){
        if(branch.name !== addedBranch.name){
          branch.selected = false;
        }
      });
    }
      $scope.branchRecipients.push(newEntry);
  };

  $scope.deselectBranch = function(branchName){
    if(branchName === 'All Users'){
      $scope.allUsersSelected = false;
    }
    $scope.selectedBranch = {};
    $scope.availableBranches.forEach(function(branch){
      if(branchName === branch.name){
        branch.selected = false;
      }
    });
  };

  $scope.removeFromRecipients = function(recipientId, recipientList) {
    recipientList.forEach(function (current, index) {
      if (recipientId === current.id){
        recipientList.splice(index, 1);
      }
    });
  };

  $scope.sendMessage = function (broadcast, customerRecipients, userRecipients, systemUpdate, link) {
    var label = systemUpdate ? 'System Update' : 'Admin Message';
    var payload = {
      customers: [],
      users: [],
      msg: {
        label: label, // ??
        subject: broadcast.subject,
        body: broadcast.bodyContent,
        link: broadcast.link,
        mandatory: false // ??
      }
    };

    if($scope.isMandatory){
      payload.msg.mandatory = true;
      var branches = '';
      if($scope.branchRecipients.length > 0){
        if(!$scope.allUsersSelected){
          //Build string of comma separated branch abbreviations
          $scope.branchRecipients.forEach(function(branch){
          branches = (branches.length === 0) ? branch.id : branches.concat(',' + branch.id);
        });

        payload.branchestoalert = branches;
        }
      }
    }
    else{
      customerRecipients.forEach(function(customer) {
        payload.customers.push(customer.id);
      });
      userRecipients.forEach(function(user) {
        payload.users.push(user.id);
      });
    }

    if($scope.isBEKSysAdmin == true && ($scope.isSystemUpdate == true || $scope.isMandatory)) {
        MessagingService.broadcastMandatoryMessage(payload).then(function (success) {
          $scope.displayMessage('success', 'The message was sent successfully.');
          $scope.resetMessageFields(); //reset message inputs
        }, function (error) {
          $scope.displayMessage('error', 'There was an error sending the message: ' + error);
        });
    } else {
        MessagingService.broadcastMessage(payload).then(function (success) {
            $scope.displayMessage('success', 'The message was sent successfully.');
            $scope.resetMessageFields(); //reset message inputs
        }, function (error) {
            $scope.displayMessage('error', 'There was an error sending the message: ' + error);
        });
    }
  };



}]);
