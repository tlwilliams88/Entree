'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:NotificationService
 * @description
 * # NotificationService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('NotificationService', [ '$http', '$filter', 'Notification', 'toaster',
    function ($http, $filter, Notification, toaster) {
    
    var Service = {
      userNotificationsCount: { }, // must be an object
      mandatoryMessages: [],
      systemUpdates: [],
      
      getUnreadMessageCount: function() {
        return $http.get('/messaging/usermessages/unreadcount').then(function(response) {
          Service.setUnreadCount(response.data.successResponse);
          return response.data.successResponse;
        });
      },

      setUnreadCount: function(count){
        angular.copy({ unread: count }, Service.userNotificationsCount); // convert int to object
      },

      getMessages: function(params) {      
        return Notification.save(params).$promise.then(function(data) {
          data.successResponse.results.forEach(function(notification) {
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
            if(notification.mandatory){
              notification.displayType = 'News';
              if($filter('filter')(Service.mandatoryMessages, {id: notification.id}).length === 0 && !notification.messageread){
                if(notification.label == 'System Update'){
                  Service.systemUpdates.push(notification);
                } else {
                  Service.mandatoryMessages.push(notification);
                }
              }
            }
          });
          return data.successResponse;
        });
      },

      updateUnreadMessages: function(messages) {
        var unreadMessages = [];

        angular.forEach(messages, function(message) {
          if (!message.messageread) {
            message.messageread = new Date();
            unreadMessages.push(message);
          }
        });

        return Notification.markAsRead().$promise.then(function() {
          Service.systemUpdates = [];
          //set unreadcount to 0 to clear notifications counter badge value
          Service.setUnreadCount(0);
        });
      },

      forwardNotification: function(notification) {
        Notification.forwardNotification(notification).$promise.then(function(resp){
          var response = resp.successResponse;

          if(response == true){
            toaster.pop('success', null, 'Successfully forwarded notification to ' + notification.emailaddress + '.');
          } else {
            toaster.pop('error', null, response.errorMessage);
          }

          return response;
          
        });
      }
    };
 
    return Service;
 
  }]);