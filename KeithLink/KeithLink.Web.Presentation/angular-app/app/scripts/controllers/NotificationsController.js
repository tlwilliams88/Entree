'use strict';

angular.module('bekApp')
  .controller('NotificationsController', ['$scope', '$modal',
    function ($scope, $modal) {
    
  $scope.notifications = [{
    'type': 'order',
    'message': 'Order #12345 has been confirmed.',
    'datetime': '2014-03-09'
  }, {
    'type': 'invoice',
    'message': 'Invoice #20123 is past due.',
    'datetime': '2014-03-09'
  }, {
    'type': 'marketing',
    'message': 'Recall on Pilgrim Chicken Breast due to E. Coli!',
    'datetime': '2014-05-12',
    'extmessage': 'Recall on Pilgrim Chicken Breast due to E. Coli! Extended message!'
  }];

  $scope.showAdditionalInfo = function(notification) {
    var modalInstance = $modal.open({
      templateUrl: 'views/notificationdetailsmodal.html',
      controller: 'NotificationDetailsModalController',
      windowClass: 'notification-modal',
      resolve: {
        notification: function() {
          return notification;
        }
      }
    });
  };

  }]);
