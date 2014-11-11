'use strict';

angular.module('bekApp')
  .controller('NotificationsController', ['$scope', '$modal', 'notifications', 'NotificationService',
    function ($scope, $modal, notifications, NotificationService) {
    
  $scope.notifications = notifications;
  $scope.sortBy = 'messagecreatedutc';
  $scope.sortOrder = true;

  // mark messages read
  NotificationService.updateUnreadMessages(angular.copy(notifications));

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
