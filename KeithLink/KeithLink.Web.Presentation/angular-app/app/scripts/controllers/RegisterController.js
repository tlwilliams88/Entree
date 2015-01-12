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
        if ( AccessService.isOrderEntryCustomer() || AccessService.isInternalUser() ) {
          $state.transitionTo('menu.home');  
        } else {
          $state.transitionTo('menu.catalog.home');
        }
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