'use strict';

angular.module('bekApp')
  .controller('NotificationsController', ['$scope', '$modal', 'notifications', 'NotificationService',
    function ($scope, $modal, notifications, NotificationService) {
    

  notifications.forEach(function(notification) {
    switch (notification.notificationtype) {
      case 0: // My order is confirmed
      case 1: // My order is shipped
        notification.displayType = 'Order';
        break;
      case 2: // My invoices need attention
        notification.displayType = 'Invoice';
        break;
      case 3: // Ben E Keith has news for me
        break;
    }
  });

  $scope.notifications = notifications;
  $scope.sortBy = 'messagecreatedutc';
  $scope.sortOrder = true;

  // mark messages read
  NotificationService.updateUnreadMessages(angular.copy(notifications));

  $scope.showAdditionalInfo = function(notification) {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/notificationdetailsmodal.html',
      controller: 'NotificationDetailsModalController',
      windowClass: 'color-background-modal',
      scope: $scope,
      resolve: {
        notification: function() {
          return notification;
        }
      }
    });
  };

  }]);
