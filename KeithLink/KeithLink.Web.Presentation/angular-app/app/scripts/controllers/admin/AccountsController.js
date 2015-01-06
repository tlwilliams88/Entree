'use strict';

angular.module('bekApp')
  .controller('AccountsController', ['$scope', 'AccountService',
    function ($scope, AccountService) {
    
    $scope.searchAccounts = function(searchTerm) {
      AccountService.searchAccounts(searchTerm).then(function(accounts) {
        $scope.accounts = accounts;
      });
    }

  }]);
