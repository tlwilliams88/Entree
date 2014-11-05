'use strict';

angular.module('bekApp')
  .controller('EditUserDetailsController', ['$scope', '$stateParams', 'UserProfileService', 'LocalStorage',
    function ($scope, $stateParams, UserProfileService, LocalStorage) {
      /*---convenience functions---*/
      var refreshProfile = function(){
        UserProfileService.getUserProfile($stateParams.email).then(
        function(returnedProfile){
          //rename email field to correct response/request mismatch <---NEEDS ENDPOINT FIXES, TEMPORARY FIX
          returnedProfile.email = returnedProfile.emailaddress;
          delete returnedProfile.emailaddress;
          $scope.profile = returnedProfile;

          //match current account customers to new profile customers
          $scope.profile.user_customers.forEach(function(profileCustomer){
            $scope.customers.forEach(function(accountCustomer){
              if(accountCustomer.customerId == profileCustomer.customerId){
                accountCustomer.selected = true;
                console.log('customer match');
              } else {
                console.log('customer didnt match');
              }
            });
          });

        },function(error){
          console.log(error);
          $scope.displayMessage('error', "An error occurred: " +  error);
        });
      };

      /*---Init---*/
      //set default table sorting
      $scope.sort = {column: 'customerNumber', descending: false};

      //get available roles <----NEEDS ENDPOINT
      $scope.roles = ["owner", "accounting", "approver", "buyer", "guest"];

      //get customers from the account of the currently logged in user
      $scope.customers = LocalStorage.getProfile().user_customers;
      console.log($scope.customers);

      //get current user profile
      refreshProfile();

      /*---selected customers functions---*/
      $scope.changeAllSelected = function (state) {
        $scope.customers.forEach(function(customer){
          customer.selected = state;
        });
      };

      /*---edit profile---*/

      $scope.updateProfile = function () {
        //strips selected property out of customer objects and creates array of just selected customers
        var cleanSelectedCustomers = [];
        $scope.customers.forEach(function(selectedCustomer){
          if(selectedCustomer.selected){
            delete selectedCustomer.selected;
            cleanSelectedCustomers.push(selectedCustomer);
          }
        });

        //attaches only clean selected users to the profile object
        $scope.profile.user_customers = cleanSelectedCustomers;

        //pushes profile object to database
        UserProfileService.updateUser($scope.profile).then(function(){
          $scope.displayMessage('success',"The user was successfully updated.");
        },function(error){
          $scope.displayMessage('error',"An error occurred: " + error);
        });
      };

      $scope.deleteProfile = function () {
        //wipe customers out of user profile
        $scope.profile.user_customers = [];

        //push freshly wiped profile to database
        UserProfileService.updateUser($scope.profile).then(function(){
          //displays message to user that the transaction was completeed successfully
          $scope.displayMessage('success',"The user was successfully deleted.");
          //refreshes data on page to match database
          refreshProfile();
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
