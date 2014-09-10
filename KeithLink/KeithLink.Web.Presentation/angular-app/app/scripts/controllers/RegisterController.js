'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:RegisterController
 * @description
 * RegisterController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('RegisterController', ['$scope', '$state', 'AuthenticationService', 'AccessService', 'BranchService', 'UserProfileService',
    function ($scope, $state, AuthenticationService, AccessService, BranchService, UserProfileService) {

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
        $scope.errorMessage = error.data.error_description;
      });

    };

    $scope.registerNewUser = function(userProfile) {
      var profile = {};
      profile.email = userProfile.email;
      profile.password = userProfile.password;
      UserProfileService.createUser(profile);
    };

}]);