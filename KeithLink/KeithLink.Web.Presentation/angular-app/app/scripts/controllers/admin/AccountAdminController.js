'use strict';

angular.module('bekApp')
  .controller('AccountAdminController', ['$scope', 'UserProfileService', 'branches', 'LocalStorage', '$state', 'CustomerService', 'AccountService',
    function ($scope, UserProfileService, branches, LocalStorage, $state, CustomerService, AccountService) {

      var accountid = '';
      //get user id from localstorage and ask for account id with it
      AccountService.getAccountByUser(LocalStorage.getProfile().userid).then(function (success) {
        accountid = success;
        console.log(success);
        //get all customers on account
        CustomerService.getCustomers('4728d73a-81d7-4262-a1b0-88fd46aade32').then(function(success){
          $scope.customers = success;

          //get all users for every customer on the account
          var tempArray = [];
          var userAdded = false;
          $scope.customers.forEach(function (customer) {
            var data = {
              params: {
                customerid: customer.customerId
              }
            };
            //request all users for a single customer
            UserProfileService.getAllUsers(data).then(function (usersOfCustomer) {
              usersOfCustomer.forEach(function (user) {
                userAdded = false;
                //check if the user already exists in the temporary array
                tempArray.forEach(function(addedUser){
                  if(addedUser.userid === user.userid){
                    //flag user as already existing
                    userAdded = true;
                  }
                 });
                  if(!userAdded) {
                    //add user to temporary array
                    tempArray.push(user);
                  }
              });
            });
          }, function (error) {
            $scope.displayMessage('error', 'An error occurred: ' + error);
          });
          $scope.users = tempArray;
        }, function(error){
          $scope.displayMessage('error', 'An error has occurred retrieving the customer list: ' + error)
        });
      }, function (error) {
        $scope.displayMessage('error', 'An error occurred while retreiving the account number: ' + error);
      });


      $scope.userExists = false;
      /*---User Profile Functions---*/
      $scope.updateUserProfile = function (userProfile) {
        userProfile.email = userProfile.emailaddress;
        $scope.updateProfileErrorMessage = null;

        UserProfileService.updateUser(userProfile).then(function (profile) {
          $scope.$parent.userProfile = profile;
          $scope.displayMessage('success', 'Successfully updated profile.');
        }, function (errorMessage) {
          $scope.updateProfileErrorMessage = errorMessage;
        });
      };

      $scope.checkUser = function (checkEmail) {
        //set email as a parameter
        var data = {
          params: {
            email: checkEmail
          }
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
              UserProfileService.createUser(newProfile).then(
                function (profile) {
                  //redirects to user profile page
                  $state.go('menu.admin.edituser', {email: checkEmail});
                }, function (errorMessage) {
                  console.log(errorMessage);
                  $scope.displayMessage('error', 'An error occurred creating the user: ' + errorMessage)
                });
            }
          }, function (errorMessage) {
            $scope.displayMessage('error', 'An error occurred checking if the user exists: ' + errorMessage);
          });
      };
    }]);
