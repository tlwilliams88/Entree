'use strict';

angular.module('bekApp')
.controller('ErrorMessageModalController', ['$scope', '$modalInstance', '$state', 'message', 'blockUI',
  function ($scope, $modalInstance, $state, message, blockUI) {

  $scope.message = message;

  $scope.accept = function () {
    $modalInstance.dismiss('cancel');
  };

  $scope.redirect = function() {
      blockUI.stop();
      $state.go('menu.home');
      $modalInstance.dismiss('cancel');
  }
}]);