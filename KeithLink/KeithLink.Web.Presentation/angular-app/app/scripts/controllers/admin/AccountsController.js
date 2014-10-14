'use strict';

angular.module('bekApp')
  .controller('AccountsController', ['$scope', 'AccountService',
    function ($scope, AccountService) {
    
    $scope.accounts = AccountService.accounts;

  }]);
