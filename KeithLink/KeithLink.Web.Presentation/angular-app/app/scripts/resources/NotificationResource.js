'use strict';

angular.module('bekApp')
  .factory('Notification', [ '$resource', 
  function ($resource) {
    return $resource('/usermessages', { }, {

      // defaults: QUERY

      update: {
        url: '/usermessages',
        method: 'PUT'
      }
      
    });
  
  }]);
