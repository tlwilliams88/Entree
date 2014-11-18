'use strict';

angular.module('bekApp')
  .controller('AddUserDetailsController', ['$scope', 'UserProfileService', 'LocalStorage',
    function ($scope, UserProfileService, LocalStorage) {
      /*------Init------*/
      //temporary hardcoded roles in use until a role endpoint is created
      $scope.userRoles = ["Owner", "Accounting", "Approver", "Buyer", "Guest"];

      $scope.customers = LocalStorage.getProfile().user_customers;

      /*------NA/RA grouping logic------*/
      var indexedGroups = [];

      //rechecks if group numbers have changed each time the page is refreshed
      $scope.customersToFilter = function() {
        indexedGroups = [];
        return $scope.customers;
      };

      //filters group numbers from customers ensuring they are unique and not null
      $scope.filterCustomers = function(customer) {
          var groupNum = customer.nationalOrRegionalAccountNumber;
          var groupIsNew = indexedGroups.indexOf(groupNum) == -1;
          if (groupIsNew && !$scope.isNullEmptyUndefined(groupNum)) {
              indexedGroups.push(groupNum);
              return groupIsNew;
          } else {
            return false;
          }
      };

      //filters out all customers with an assigned group to populate the unassigned customer list
      $scope.filterOutGroups = function(customer) {
        return !!$scope.isNullEmptyUndefined(customer.nationalOrRegionalAccountNumber);
      };

      /*------Selected customers logic------*/
      $scope.selectedCustomers = [];

      //updates the selected list each time a selection has changed
      $scope.updateSelectedList = function(){
        $scope.selectedCustomers = [];
        $scope.customers.forEach(function(customer){
          if(customer.selected) {
            $scope.selectedCustomers.push(customer);
          }
        });
      };

      //updates all customers in a group to the same selection status
      $scope.selectAllCustomersInGroup = function(groupNumber, groupSelectionStatus){
        $scope.customers.forEach(function(customer){
          if(customer.nationalOrRegionalAccountNumber == groupNumber){
            customer.selected = groupSelectionStatus;
          }
        });
        $scope.updateSelectedList();
      };

      /*------User Logic------*/
      /*Create vs Find User*/
      $scope.isExistingUser = false;

      //checks if user exists
      $scope.checkUser = function(){
        //set email as a parameter
        var data = {
          params: {
            email: $scope.currentUserEmail
          }
        };

        //check if user exists in the database
        UserProfileService.getAllUsers(data).then(
          function(profile){
            //if the user does exist update userExists flag to true, else keep it as false
            if(!$scope.isNullEmptyUndefined(profile)){

              //RENAMING EMAIL ADDRESS BECAUSE RESPONSE RETURNS INCORRECT FIELD NAME
              profile[0].email = profile[0].emailaddress;
              delete profile[0].emailaddress;

              $scope.currentSelectedUser = profile[0];
              $scope.isExistingUser = true;
            } else {
              $scope.isExistingUser = false;
            }
          }, function(errorMessage){
            console.log(errorMessage);
          });
      };

      $scope.makeUser= function(){
        if(!$scope.isExistingUser){
          //creates new user profile object
          var newProfile = {};
          newProfile.email = $scope.currentUserEmail;
          if($scope.isNullEmptyUndefined($scope.selectedCustomers[0].customerBranch)){
            newProfile.branchId = "";
          } else {
            newProfile.branchId = $scope.selectedCustomers[0].customerBranch;
          }

          //sends new User Profile to db and receives newly generated profile object
          UserProfileService.createUser(newProfile).then(function(profile){
            $scope.currentSelectedUser = profile.userProfiles[0];
          updateExistingProfile();
          },function(errorMessage){
            console.log(errorMessage);
          });
        } else {
          updateExistingProfile();
        }
      };

      var updateExistingProfile = function(){
        //strip SELECTED property from object to prevent errors with API
        var noSelectCustomers = [];
        $scope.selectedCustomers.forEach(function(selectedCustomer){
          delete selectedCustomer.selected;
          noSelectCustomers.push(selectedCustomer);
        });

        //updates existing users customers with currently selected customers
        $scope.currentSelectedUser.rolename = $scope.currentUserRole;
        $scope.currentSelectedUser.user_customers = noSelectCustomers;

        //send updated profile to endpoint
        UserProfileService.updateUser($scope.currentSelectedUser).then(function(){
          $scope.displayMessage('success',"The user was successfully added.");
          $scope.wipeForm();
        },function(error){
          $scope.displayMessage('error',"An error occurred: " + error);
        });
      };

      /*------Convenience Methods------*/
      //allows for proper checking of empty group numbers
      $scope.isNullEmptyUndefined = function(val){
        return !!(angular.isUndefined(val) || val === null || val.length == 0);
      };

      $scope.wipeForm = function(){
        $scope.addUserForm.$setPristine(true);
        $scope.currentUserEmail = "";
        $scope.isExistingUser = false;
        $scope.currentUserRole = $scope.userRoles[4];
        $scope.currentSelectedUser = {};
        $scope.selectedCustomers = [];
        $scope.groupChecked = false;
        $scope.customers.forEach(function(customer){
          customer.selected = false;
        });
      };

    }]
  );
