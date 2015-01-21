'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:RegisterController
 * @description
 * RegisterController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('RegisterController', ['$scope', '$state', 'ENV', 'AuthenticationService', 'AccessService', 'BranchService', 'UserProfileService',
    function ($scope, $state, ENV, AuthenticationService, AccessService, BranchService, UserProfileService) {

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
<<<<<<< HEAD
=======
        if (ENV.mobileApp) { // ask to allow push notifications
          PhonegapPushService.register();
        }
>>>>>>> 1199270747403d434d230fb61b127bed91e83642
        $scope.redirectUserToCorrectHomepage();
      }, function(errorMessage) {
        $scope.loginErrorMessage = errorMessage;
      });

    };

    $scope.registerNewUser = function(userProfile) {
      $scope.registrationErrorMessage = null;
      
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