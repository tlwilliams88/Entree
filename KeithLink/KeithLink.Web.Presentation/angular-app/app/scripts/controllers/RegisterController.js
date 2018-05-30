'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:RegisterController
 * @description
 * RegisterController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('RegisterController', ['$scope', '$state', 'ENV', 'toaster', 'AuthenticationService', 'AccessService', 'BranchService', 'UserProfileService', 'PhonegapPushService', 'LocalStorage', 'Constants', '$window', 'localStorageService', 'blockUI', '$interval',
    function ($scope, $state, ENV, toaster, AuthenticationService, AccessService, BranchService, UserProfileService, PhonegapPushService, LocalStorage, Constants, $window, localStorageService, blockUI, $interval) {

  $scope.isMobileApp = ENV.mobileApp;
  $scope.signUpBool = false;
  $scope.isInternalEmail = false;
  $scope.defaultUserName = ENV.username;
  $scope.saveUserName = $scope.defaultUserName ? true : false;

  // gets prepopulated login info for dev environment
  if(ENV.username) {
    $scope.loginInfo = {
      username: $scope.defaultUserName,
      password: ''
    };
  } else {
    $scope.loginInfo = {
      username: '',
      password: ''
    };
  }

  $scope.setSavingUserName = function(username) {
    $scope.saveUserName = !$scope.saveUserName;
    $scope.enteredUserName = username;
  };

  var branchCheck;
  function checkForBranches() {
    BranchService.getBranches().then(function(resp){
      if(resp == -1) {
        return;
      } else {
        blockUI.stop();
        $interval.cancel(branchCheck);
        $scope.branches = resp.successResponse;
      }
    })
  }

  BranchService.getBranches().then(function(resp) {
    var branches = [],
        maintenanceMessage = 'We\'re currently undergoing maintenance for an extended period today.\n We\'ll be back soon.\n Thank you for your patience.';
    if(resp == -1 && ENV.isMobileApp) {
      blockUI.start(maintenanceMessage).then(function() {
        branchCheck = $interval(checkForBranches, 30000);
      })
    } else {
      if(resp && resp.length > 0) {
        branches = resp;
      }
      $scope.branches = branches;
    }
  });

  $scope.login = function(loginInfo) {
    $scope.loginErrorMessage = '';

    if($scope.saveUserName){
      LocalStorage.setDefaultUserName(loginInfo.username);
    } else {
      LocalStorage.setDefaultUserName('');
    }

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
    $scope.checkForInternalEmail(email);
    if(!$scope.isInternalEmail){
      UserProfileService.resetPassword(email).then(function(data){
       toaster.pop('success', null, 'We have sent a password reset request if the email was verified');
      },function(error) {
      toaster.pop('error', null, 'Error resetting password.');
      });
    } else {
      return false;
    }

   };

  $scope.setSignUpBool = function(signUpBool) {
    $scope.registerUser.password = null;

    $scope.registerUser.email = null;

    $scope.signUpBool = !signUpBool;
  };

  $scope.checkForInternalEmail = function(email) {
    if(email.slice(email.indexOf('@'),(email.indexOf('@') + 14)).toLowerCase(0) === "@benekeith.com"){
      $scope.isInternalEmail = true;
      $scope.$digest();
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
    $scope.registrationErrorMessage = null;
    $scope.signUpBool = false;
  };

}]);
