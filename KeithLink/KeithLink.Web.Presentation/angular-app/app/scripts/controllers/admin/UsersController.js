'use strict';

angular.module('bekApp')
  .controller('UsersController', ['$scope', 'users',
    function ($scope, users) {
    
    $scope.users = users;

  }]);
