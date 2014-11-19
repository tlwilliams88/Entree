'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:NotificationService
 * @description
 * # NotificationService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('NotificationService', [ '$http', 'Notification',
    function ($http, Notification) {
    
    var Service = {
      userNotifications: { }, // must be an object

      getUnreadMessageCount: function() {
        return $http.get('/usermessages/unreadcount').then(function(response) {
          angular.copy({ unread: response.data }, Service.userNotifications); // convert int to object
          return response.data;
        });
      },

      getMessages: function(params) {
        return Notification.save(params).$promise.then(function(data) {
          data.results.forEach(function(notification) {
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
          return data;
        });
      },

      updateUnreadMessages: function(messages) {
        var unreadMessages = [];

        angular.forEach(messages, function(message) {
          if (!message.messagereadutc) {
            message.messagereadutc = new Date();
            unreadMessages.push(message);
          }
        });

        return Notification.markAsRead(unreadMessages).$promise.then(function() {
          angular.copy({ unread: '0' }, Service.userNotifications);
        });
      }
    };
 
    return Service;
 
  }]);