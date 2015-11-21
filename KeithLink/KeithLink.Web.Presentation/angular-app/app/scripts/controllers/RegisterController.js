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

    $scope.signUpBool = false;

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
    
    AuthenticationService.login(loginInfo.username, loginInfo.password)
      .then(UserProfileService.getCurrentUserProfile)
      .then(function(profile) {
        if (ENV.mobileApp) { // ask to allow push notifications
          PhonegapPushService.register();
        }
        $scope.redirectUserToCorrectHomepage();
      }, function(errorMessage) {
        $scope.loginErrorMessage = errorMessage;
      });

  };

  $scope.forgotPassword = function(email) { 
    UserProfileService.resetPassword(email).then(function(data){      
       toaster.pop('success', null, 'We have sent a password reset request if the email was verified');
      },function(error) {
      toaster.pop('error', null, 'Error resetting password.');        
      });
   };

  $scope.setSignUpBool = function(signUpBool) { 
    $scope.signUpBool = !signUpBool;
  };

  $scope.checkForInternalEmail = function(email) {    
    if(email.slice(email.indexOf('@'),(email.indexOf('@') + 14)).toLowerCase(0) === "@benekeith.com"){
      $scope.isInternalEmail = true;
    }else{
      $scope.isInternalEmail = false;
    }
  };

  var processingRegistration = false;
  $scope.registerNewUser = function(userProfile) {
    if (!processingRegistration) {
      processingRegistration = true;
      $scope.registrationErrorMessage = null;
      
      UserProfileService.createUser(userProfile).then(function(data) {

        // log user in
        AuthenticationService.login(userProfile.email, userProfile.password)
          .then(UserProfileService.getCurrentUserProfile)
          .then(function(profile) {
            // redirect to account details page
            $state.go('menu.userprofile');
          }, function(error) {
            $scope.loginErrorMessage = error.data.error_description;
            $scope.clearForm();
            $scope.loginInfo = {};
          });
        }, function(error) {
          $scope.registrationErrorMessage = error;
        }).finally(function() {
          processingRegistration = false;
        });
    }
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