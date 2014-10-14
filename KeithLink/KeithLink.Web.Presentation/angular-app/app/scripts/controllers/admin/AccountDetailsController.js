'use strict';

angular.module('bekApp')
  .controller('AdminAccountDetailsController', ['$scope', '$stateParams', 'AccountService',
    function ($scope, $stateParams, AccountService) {
    
    if ($stateParams.accountId === 'new') {
      $scope.isNew = true;
    } else {
      $scope.account = AccountService.findAccountById($stateParams.accountId);
      $scope.isNew = false;
    }

    $scope.createNewAccount = function(account) {
      AccountService.createAccount(account).then(function(accounts) {
        $scope.displayMessage('success', 'Successfully created a new account.');
      }, function(error) {
        console.log(error);
        $scope.displayMessage('error', 'Error creating new account.');
      });
    };

  }]);
