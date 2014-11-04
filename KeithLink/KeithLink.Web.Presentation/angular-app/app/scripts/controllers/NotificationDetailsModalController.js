'use strict';

angular.module('bekApp')
.controller('NotificationDetailsModalController', ['$scope', '$modalInstance', 'notification',
  function ($scope, $modalInstance, notification) {

  $scope.notification = notification;

  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

}]);