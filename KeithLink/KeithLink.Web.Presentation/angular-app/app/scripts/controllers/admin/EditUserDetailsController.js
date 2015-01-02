'use strict';

angular.module('bekApp')
  .controller('EditUserDetailsController', ['$scope', '$stateParams', 'UserProfileService', 'LocalStorage', 'returnedProfile', 'AccountService', 'CustomerService',
    function ($scope, $stateParams, UserProfileService, LocalStorage, returnedProfile, AccountService, CustomerService) {
      /*---convenience functions---*/
      var processProfile = function(newProfile){
        //rename email <----- NEEDS FIX ON RESPONSE TYPE
        newProfile.email = newProfile.emailaddress;
        delete newProfile.emailaddress;

        //rename role <----- NEEDS FIX ON RESPONSE TYPE
        newProfile.role = newProfile.rolename;
        delete newProfile.rolename;

        //rename customers <----- NEEDS FIX ON RESPONSE TYPE
        newProfile.customers = newProfile.user_customers;
        delete newProfile.user_customers;

        $scope.profile = newProfile;

        //if the user has customers assigned to them, go through the account customers and set matching
        //ones to true, otherwise set to false
        if($scope.profile.customers){
          $scope.profile.customers.forEach(function(profileCustomer){
            $scope.customers.forEach(function(accountCustomer){
              if(accountCustomer.customerNumber === profileCustomer.customerNumber){
                accountCustomer.selected = true;
              } else {
                if(!accountCustomer.selected === true) {
                  accountCustomer.selected = false;
                }
              }
            });
          });
        } else {
          $scope.customers.forEach(function(customer){
            customer.selected = false;
          });
        }
      };

      //logic for proper select filtering, allows user to disable filter instead of showing only true or only false
      $scope.filterFields = {};
      $scope.setSelectedFilter = function(selectedFilter) {
        if (selectedFilter) {
          delete $scope.filterFields.selected;
        } else {
          $scope.filterFields.selected = true;
        }
      };

      /*---Init---*/
      //set default table sorting
      $scope.sortBy = 'customerNumber';
      $scope.sortOrder = true;

      //$scope.roles = RoleService.getRoles(); //get available roles <----NEEDS ENDPOINT

      $scope.roles = ["owner", "accounting", "approver", "buyer", "guest"];

      //get customers from the account of the currently logged in user
      var accountid = '';
      AccountService.getAccountByUser(LocalStorage.getProfile().userid).then(function (success) {
        accountid = success.id;
        //get all customers on account
        CustomerService.getCustomers(accountid).then(function (success) {
          $scope.customers = success;
          //get current user profile
          processProfile(returnedProfile);
        })
      });

      /*---selected customers functions---*/
      $scope.changeAllSelected = function (state) {
        $scope.customers.forEach(function(customer){
          customer.selected = state;
        });
      };

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
          $scope.displayMessage('success',"The user was successfully updated.");
          //processProfile(newProfile); // <-- UNCOMMENT WHENEVER DATA SENT BACK IS FRESH
        },function(error){
          $scope.displayMessage('error',"An error occurred: " + error);
        });
      };

      $scope.deleteProfile = function () {
        //wipe customers out of user profile and set profile to lowest permission role
        $scope.profile.role = "guest";
        $scope.profile.customers = [];

        //push freshly wiped profile to database
        UserProfileService.updateProfile($scope.profile).then(function(newProfile){
          //refreshes page with newest data
          processProfile(newProfile);
          //displays message to user that the transaction was completed successfully
          $scope.displayMessage('success',"The user was successfully deleted.");
        },function(error){
          $scope.displayMessage('error',"An error occurred: " + error);
        });
      };

      /*---Sorting Controls for Table---*/
      $scope.changeSort = function (column) {
        var sort = $scope.sort;
        if (sort.column == column) {
            sort.descending = !sort.descending;
        } else {
            sort.column = column;
            sort.descending = false;
        }
      };
  }]);
