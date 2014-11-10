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

      getAllMessages: function() {
        return Notification.query().$promise;
      },

      updateUnreadMessages: function(messages) {
        var unreadMessages = [];

        angular.forEach(messages, function(message) {
          if (!message.messagereadutc) {
            message.messagereadutc = new Date();
            unreadMessages.push(message);
          }
        });

        return Notification.update(unreadMessages).$promise.then(function() {
          angular.copy({ unread: '0' }, Service.userNotifications);
        });
      }
    };
 
    return Service;
 
  }]);