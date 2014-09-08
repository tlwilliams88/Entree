'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:RegisterController
 * @description
 * RegisterController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('RegisterController', ['$scope', '$state', 'AuthenticationService', 'BranchService',
    function ($scope, $state, AuthenticationService, BranchService) {

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
        $state.transitionTo('menu.home');
      }, function(error) {
        $scope.errorMessage = error.data.error_description;
      });

    };

}]);