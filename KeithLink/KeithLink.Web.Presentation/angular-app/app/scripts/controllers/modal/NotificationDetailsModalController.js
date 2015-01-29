'use strict';

angular.module('bekApp')
.controller('NotificationDetailsModalController', ['$scope', '$modalInstance', '$sce', '$state', 'notification',
  function ($scope, $modalInstance, $sce, $state, notification) {

  $scope.notification = notification;

  $scope.notification.body = $sce.trustAsHtml(notification.body);

  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

  $scope.goToPage = function(stateName) {
    $modalInstance.close();
    $state.go(stateName);
  };

}]);