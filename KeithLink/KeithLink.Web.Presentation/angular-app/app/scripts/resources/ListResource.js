'use strict';

angular.module('bekApp')
  .factory('List', [ '$resource', 'UserProfileService', 
  function ($resource, UserProfileService) {
    return $resource('/list/:branchId/:listId', { }, {

      // defaults: GET, QUERY, SAVE

      update: {
        url: '/list',
        method: 'PUT'
      },

      delete: {
        url: '/list/:listId',
        method: 'DELETE'
      },

      addItem: {
        url: '/list/:listId/item',
        method: 'POST'
      },

      updateItem: {
        url: '/list/:listId/item',
        method: 'PUT'
      },

      deleteItem: {
        url: '/list/:listId/item/:listItemId',
        method: 'DELETE'
      }

    });
  
  }]);
