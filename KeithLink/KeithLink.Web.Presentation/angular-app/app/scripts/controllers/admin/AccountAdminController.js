'use strict';

angular.module('bekApp')
  .controller('AccountAdminController', ['$scope', 'UserProfileService', 'branches', 'LocalStorage', '$state',
    function ($scope, UserProfileService, branches, LocalStorage, $state) {
      $scope.customers = $scope.userProfile.user_customers;
      //TEST DATA UNTIL ENDPOINT EXISTS
      $scope.users = [
        {'email': 'rhedges@credera.com', 'firstName': 'Robert', 'lastName': 'Hedges', 'role': 'owner'},
        {'email': 'chendon@credera.com', 'firstName': 'Clay', 'lastName': 'Hendon', 'role': 'buyer'},
        {'email': 'aallen@credera.com', 'firstName': 'Andrew', 'lastName': 'Allen', 'role': 'guest'},
        {'email': 'jshields@credera.com', 'firstName': 'John', 'lastName': 'Shields', 'role': 'owner'},
        {'email': 'gsalazar@credera.com', 'firstName': 'Gabe', 'lastName': 'Salazar', 'role': 'accounting'}
      ];
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
