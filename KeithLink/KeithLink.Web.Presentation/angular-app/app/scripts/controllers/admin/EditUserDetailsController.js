'use strict';

angular.module('bekApp')
  .controller('EditUserDetailsController', ['$scope', '$state', '$stateParams', '$filter', 'UserProfileService', 'userProfile', 'userCustomers', 'CustomerPagingModel',
    function ($scope, $state, $stateParams, $filter, UserProfileService, userProfile, userCustomers, CustomerPagingModel) {

  $scope.groupId = $stateParams.groupId;
  
  function checkIfUserExistsOnAnotherGroup() {
    // add check if userCustomers are in a different customer group
    // throw warning to user that assigning different custoemr will change the customer group
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
        customer.isChecked = false;
      }
    });
    return customers;
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
  customerPagingModel.accountId = $scope.groupId; 

  // TODO: get available roles <----NEEDS ENDPOINT
  $scope.roles = ['owner', 'accounting', 'approver', 'buyer', 'guest'];

  processProfile(userProfile);
  customerPagingModel.loadCustomers()
    .then(checkIfUserExistsOnAnotherGroup);

  /**********
  FORM EVENTS
  **********/

  $scope.updateProfile = function () {
    //attaches only selected customers to profile object before it is pushed to the database
    var selectedCustomers = [];
    $scope.customers.forEach(function(customer){
      if(customer.selected) {
        selectedCustomers.push(customer);
      }
    });

    $scope.profile.customers = selectedCustomers;

    //pushes profile object to database
    UserProfileService.updateUserProfile($scope.profile).then(function(newProfile){
      if ($scope.$parent.$parent.userProfile.userid === newProfile.userid) {
        $scope.$parent.$parent.userProfile = newProfile;
      }
      $scope.displayMessage('success', 'The user was successfully updated.');
    }, function(error){
      $scope.displayMessage('error', 'An error occurred: ' + error);
    });
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

  $scope.addSelected = function() {
   $scope.editUserForm.$setDirty();
    $scope.customers.forEach(function(customer) {
      if(customer.isChecked){    
        customer.isChecked = false;
        customer.selected = true;
         $scope.profile.customers.push(angular.copy(customer));  
      }
    })    
      if($scope.filteredCustomers.length<30 || $scope.allAvailableSelected ){
        $scope.infiniteScrollLoadMore();
      }
       $scope.allAvailableSelected = $scope.allRemovableSelected = false;
  };

    $scope.selectAll = function(allSelected, source){
    if(source=='add'){
      $scope.customers.forEach(function(customer) {
        if(customer.selected == false){
       customer.isChecked = allSelected;
     }
      })
    }
    if(source=='remove'){
      $scope.profile.customers.forEach(function(availableCustomer) {
        availableCustomer.isChecked = allSelected;
      })
    }
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

  $scope.removeSelected = function(selectedCustomer) {
    $scope.editUserForm.$setDirty();
    $scope.foundMatch = false;
    $scope.profile.customers.forEach(function(availableCust){
        if(availableCust.isChecked){   
            $scope.customers.forEach(function(customer) {
                if (availableCust.customerNumber === customer.customerNumber) {
                  $scope.foundMatch = true;                                  
                  customer.selected  = customer.isChecked = false;
                  availableCust.selected  = availableCust.isChecked = false;
                }
            });
            if(!$scope.foundMatch){
              $scope.unselectCustomer(selectedCustomer);
            } 
        }  
    })
     $scope.profile.customers = $filter('filter')($scope.profile.customers, {selected: 'true'});
     $scope.allAvailableSelected = $scope.allRemovableSelected = false;  
  };

}]);
