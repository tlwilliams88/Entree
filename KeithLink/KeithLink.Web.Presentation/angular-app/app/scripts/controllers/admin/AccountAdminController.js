'use strict';

angular.module('bekApp')
  .controller('AccountAdminController', ['$scope', 'UserProfileService', 'branches', 'LocalStorage', '$state', 'CustomerService', 'AccountService', 'BroadcastService',
    function ($scope, UserProfileService, branches, LocalStorage, $state, CustomerService, AccountService, BroadcastService) {
      var accountid = '';
      //get user id from localstorage and ask for account id with it
      AccountService.getAccountByUser(LocalStorage.getProfile().userid).then(function (success) {
        accountid = success.id;
        //get all customers on account
        CustomerService.getCustomers(accountid).then(function(success){
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
    }]);
