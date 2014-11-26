'use strict';

angular.module('bekApp')
.controller('NotificationDetailsModalController', ['$scope', '$modalInstance', '$state', 'notification',
  function ($scope, $modalInstance, $state, notification) {

  $scope.notification = notification;

  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

  $scope.goToPage = function(stateName) {
    $modalInstance.close();
    $state.go(stateName);
  };

}]);