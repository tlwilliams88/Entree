'use strict';

angular.module('bekApp')
  .controller('AdminAccountDetailsController', ['$scope', '$state', '$stateParams', 'AccountService',
    function ($scope, $state, $stateParams, AccountService) {
    
    if ($stateParams.accountId === 'new') {
      $scope.isNew = true;
    } else {
      $scope.accountOriginal = AccountService.findAccountById($stateParams.accountId);
      $scope.account = angular.copy($scope.accountOriginal);
      $scope.isNew = false;
    }

    $scope.createNewAccount = function(account) {
      AccountService.createAccount(account).then(function(accounts) {
        // TODO: redirect to new account details page
        $scope.displayMessage('success', 'Successfully created a new account.');
      }, function(error) {
        $log.debug(error);
        $scope.displayMessage('error', 'Error creating new account.');
      });
    };

    $scope.updateAccount = function(account) {

    };

    $scope.cancelChanges = function() {
      $state.go('menu.admin.account');
    };

  }]);
