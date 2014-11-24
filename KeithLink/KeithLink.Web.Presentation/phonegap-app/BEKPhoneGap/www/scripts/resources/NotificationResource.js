'use strict';

angular.module('bekApp')
  .factory('Notification', [ '$resource', 
  function ($resource) {
    return $resource('/messaging/usermessages', { }, {

      // defaults: SAVE

      markAsRead: {
        url: '/messaging/usermessages/markasread',
        method: 'PUT'
      }
      
    });
  
  }]);
