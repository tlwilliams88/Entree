'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:RegisterController
 * @description
 * RegisterController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('RegisterController', ['$scope', '$state', 'toaster', 'AuthenticationService', 'AccessService', 'BranchService', 'UserProfileService',
    function ($scope, $state, toaster, AuthenticationService, AccessService, BranchService, UserProfileService) {

    $scope.loginInfo = {
      username: 'sabroussard@somecompany.com',
      password: 'L1ttleStev1e'
    };

    BranchService.getBranches().then(function(branches) {
      $scope.branches = branches;
    });

    $scope.login = function(loginInfo) {
      $scope.errorMessage = '';
      
      AuthenticationService.login(loginInfo.username, loginInfo.password).then(function(profile) {
        if ( AccessService.isOrderEntryCustomer() ) {
          $state.transitionTo('menu.home');  
        } else {
          $state.transitionTo('menu.catalog.home');
        }
      }, function(error) {
        $scope.loginErrorMessage = error.data.error_description;
      });

    };

    $scope.registerNewUser = function(userProfile) {
      var profile = userProfile;
      profile.branchid = userProfile.branch.id;
      
      // $scope.registrationFormSubmitted = true;
      $scope.registrationErrorMessage = null;
      
      UserProfileService.createUser(profile).then(function(data) {

        if (data.successResponse) {
          $scope.loginInfo = {};
          $scope.clearForm();
          // $scope.registrationFormSubmitted = false;

          toaster.pop('success', null, 'Successfully registered! Please log in.');
        } else {
          $scope.registrationErrorMessage = data.errorMessage;
        }
      }, function(error) {
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