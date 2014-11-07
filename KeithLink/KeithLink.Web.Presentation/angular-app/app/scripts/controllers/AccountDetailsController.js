'use strict';

angular.module('bekApp')
  .controller('AccountDetailsController', ['$scope', 'UserProfileService', 'branches', 'LocalStorage', '$state',
    function ($scope, UserProfileService, branches, LocalStorage, $state) {
      /*---init---*/
      $scope.userProfile = angular.copy(LocalStorage.getProfile());
      $scope.branches = branches;
      $scope.customers = $scope.userProfile.user_customers;
      //TEST DATA UNTIL ENDPOINT EXISTS
      $scope.users = [
        {'email':'rhedges@credera.com','firstName':'Robert', 'lastName':'Hedges'},
        {'email':'chendon@credera.com','firstName':'Clay', 'lastName':'Hendon'},
        {'email':'aallen@credera.com','firstName':'Andrew', 'lastName':'Allen'},
        {'email':'jshields@credera.com','firstName':'John', 'lastName':'Shields'},
        {'email':'gsalazar@credera.com','firstName':'Gabe', 'lastName':'Salazar'}
      ];
      $scope.userExists = false;

      /*---User Profile Functions---*/
      $scope.updateUserProfile = function(userProfile) {
        userProfile.email = userProfile.emailaddress;
        $scope.updateProfileErrorMessage = null;

        UserProfileService.updateUser(userProfile).then(function(profile) {
          $scope.$parent.userProfile = profile;
          $scope.displayMessage('success', 'Successfully updated profile.');
        }, function(errorMessage) {
          $scope.updateProfileErrorMessage = errorMessage;
        });
      };

      $scope.cancelChanges = function() {
        $scope.userProfile = angular.copy(LocalStorage.getProfile());
        $scope.updateProfileForm.$setPristine();
      };

      $scope.changePassword = function(changePasswordData) {
        $scope.changePasswordErrorMessage = null;
        changePasswordData.email = $scope.userProfile.emailaddress;

        UserProfileService.changePassword(changePasswordData).then(function(successMessage) {
          $scope.changePasswordData = {};
          $scope.changePasswordForm.$setPristine();
          $scope.displayMessage('success', 'Successfully changed password.');
        }, function(errorMessage) {
          $scope.changePasswordErrorMessage = errorMessage;
          $scope.displayMessage('error', 'Error updating profile.');
        });
      };

      $scope.savePreferences = function(){
        console.log('savePreferences');
      };

      $scope.checkUser = function(checkEmail){
        //set email as a parameter
        var data = {
          params: {
            email: checkEmail
          }
        };
        //check if user exists in the database
        UserProfileService.getAllUsers(data).then(
          function(profile){
            //if the user does exist update userExists flag to true, else keep it as false
            if(profile.length){
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
                function(profile){
                  //redirects to user profile page
                  $state.go('menu.admin.edituser', {email : checkEmail});
                },function(errorMessage){
                  console.log(errorMessage);
                  $scope.displayMessage('error', 'An error occurred creating the user: ' + errorMessage)
              });
            }
          }, function(errorMessage){
            $scope.displayMessage('error', 'An error occurred checking if the user exists: ' + errorMessage);
          });
      };
  }]);
