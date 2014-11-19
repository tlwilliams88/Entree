'use strict';

angular.module('bekApp')
  .factory('Notification', [ '$resource', 
  function ($resource) {
    return $resource('/usermessages', { }, {

      // defaults: SAVE

      markAsRead: {
        url: '/usermessages/markasread',
        method: 'PUT'
      }
      
    });
  
  }]);
