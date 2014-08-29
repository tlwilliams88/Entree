'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:RegisterController
 * @description
 * RegisterController
 * Controller of the bekApp
 */
angular.module('BEKPhoneGap')
  .controller('RegisterController', ['$scope', '$state', 'AuthenticationService', 
    function ($scope, $state, AuthenticationService) {

    $scope.loginInfo = {
      username: 'sabroussard@somecompany.com',
      password: 'L1ttleStev1e'
    };

    $scope.login = function(loginInfo) {

      AuthenticationService.login(loginInfo.username, loginInfo.password).then(function(profile) {
        $state.transitionTo('menu.home');
      });

    };

}]);