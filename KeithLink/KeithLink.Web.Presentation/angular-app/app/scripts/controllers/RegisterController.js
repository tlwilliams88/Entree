'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:RegisterController
 * @description
 * RegisterController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('RegisterController', ['$scope', '$state', 'ENV', 'toaster', 'AuthenticationService', 'AccessService', 'BranchService', 'UserProfileService', 'PhonegapPushService',
    function ($scope, $state, ENV, toaster, AuthenticationService, AccessService, BranchService, UserProfileService, PhonegapPushService) {

    $scope.isMobileApp = ENV.mobileApp;

    if($scope.isMobileApp){
      $scope.signUpBool=false;
    }
    else{
      $scope.signUpBool=true;
    }


    // gets prepopulated login info for dev environment
    $scope.loginInfo = {
      username: ENV.username,
      password: ENV.password
    };

    BranchService.getBranches().then(function(branches) {
      $scope.branches = branches;
    });

    $scope.login = function(loginInfo) {
      $scope.loginErrorMessage = '';
      
      AuthenticationService.login(loginInfo.username, loginInfo.password).then(function(profile) {
        if (ENV.mobileApp) { // ask to allow push notifications
          PhonegapPushService.register();
        }
        $scope.redirectUserToCorrectHomepage();
      }, function(errorMessage) {
        $scope.loginErrorMessage = 'Error authenticating user.';
        if (errorMessage) {
          $scope.loginErrorMessage = errorMessage;  
        }
      });

    };

  $scope.forgotPassword = function(email) { 
    UserProfileService.resetPassword(email).then(function(data){      
       toaster.pop('success', null, 'Successfully reset password.');
      },function(error) {
      toaster.pop('error', null, 'Error resetting password.');        
      });
   };

  $scope.setSignUpBool = function(signUpBool) { 
    $scope.signUpBool = !signUpBool;
   };

    $scope.registerNewUser = function(userProfile) {
      $scope.registrationErrorMessage = null;

    if(!UserProfileService.checkEmailLength(userProfile.email)){
      return;
    }
      
      UserProfileService.createUser(userProfile).then(function(data) {

        // log user in
        AuthenticationService.login(userProfile.email, userProfile.password).then(function(profile) {
          // redirect to account details page
          $state.go('menu.userprofile');
        }, function(error) {
          $scope.loginErrorMessage = error.data.error_description;
          $scope.clearForm();
          $scope.loginInfo = {};
        });

      }, function(error) {
        $scope.registrationErrorMessage = error;
      });
    };

    $scope.clearForm = function() {
      $scope.registerUser = {
        email: null,
        confirmEmail: null,
        password: null,
        confirmPassword: null,
        existingcustomer: false,
        marketingflag: true,
        branch: null
      };
      $scope.registrationForm.$setPristine();
    };

}]);