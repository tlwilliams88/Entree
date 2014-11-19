'use strict';

angular.module('bekApp')
  .factory('Notification', [ '$resource', 
  function ($resource) {
    return $resource('/usermessages', { }, {

      // defaults: GET

//       "size": 50,
// "from": 4,
// "sort": {
// "sfield": "name",
// "sdir": "asc"
// },
// "filter": [
// {
// "ffield": "test",
// "fvalue": "TestValue"
// }
// ]
// }

      markAsRead: {
        url: '/usermessages/markasread',
        method: 'PUT'
      }
      
    });
  
  }]);
