'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:NotificationService
 * @description
 * # NotificationService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('NotificationService', [ '$http', '$filter', 'Notification',
    function ($http, $filter, Notification) {
    
    var Service = {
      userNotificationsCount: { }, // must be an object
      mandatoryMessages: [],
      
      getUnreadMessageCount: function() {
        return $http.get('/messaging/usermessages/unreadcount').then(function(response) {
          angular.copy({ unread: response.data }, Service.userNotificationsCount); // convert int to object
          return response.data;
        });
      },

      getMessages: function(params) {      
        return Notification.save(params).$promise.then(function(data) {
          data.results.forEach(function(notification) {
            switch (notification.notificationtype) {
              case 0: // My order is confirmed
              case 1: // My order is shipped
              case 32://ETA
                notification.displayType = 'Order';
                break;
              case 64: //Payment confirmation
              case 2: // My invoices need attention
              case 4: // My invoices need attention
                notification.displayType = 'Invoice';
                break;
              case 3: // Ben E Keith has news for me
              case 8: // Ben E Keith has news for me
               notification.displayType = 'News';
               break;
              case 16: //Mail
              notification.displayType = 'Mail';
              break;
            }
            if(notification.mandatory && $filter('filter')(Service.mandatoryMessages, {id: notification.id}).length === 0 && !notification.messagereadutc){
             Service.mandatoryMessages.push(notification);
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
          angular.copy({ unread: Service.userNotificationsCount.unread - unreadMessages.length }, Service.userNotificationsCount);
        });
      }
    };
 
    return Service;
 
  }]);