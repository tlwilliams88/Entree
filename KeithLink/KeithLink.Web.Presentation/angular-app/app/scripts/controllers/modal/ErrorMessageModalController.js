'use strict';

angular.module('bekApp')
.controller('ErrorMessageModalController', ['$scope', '$modalInstance', '$state', 'message',
  function ($scope, $modalInstance, $state, message) {

  $scope.message = message;

  $scope.accept = function () {
    $modalInstance.close(true);
  };

  $scope.redirect = function() {     
    $modalInstance.close(false);   
  }
}]);