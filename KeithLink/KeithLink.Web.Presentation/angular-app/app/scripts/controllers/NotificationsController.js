'use strict';

angular.module('bekApp')
  .controller('NotificationsController', ['$scope', '$modal', 'notifications',
    function ($scope, $modal, notifications) {
    
  $scope.notifications = notifications;
  $scope.sortBy = 'messagecreatedutc';
  $scope.sortOrder = true;

  $scope.showAdditionalInfo = function(notification) {
    var modalInstance = $modal.open({
      templateUrl: 'views/notificationdetailsmodal.html',
      controller: 'NotificationDetailsModalController',
      windowClass: 'color-background-modal',
      resolve: {
        notification: function() {
          return notification;
        }
      }
    });
  };

  }]);
