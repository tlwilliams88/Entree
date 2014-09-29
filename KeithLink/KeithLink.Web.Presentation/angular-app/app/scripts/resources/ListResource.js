'use strict';

angular.module('bekApp')
  .factory('List', [ '$resource', 'UserProfileService', 
  function ($resource, UserProfileService) {
    return $resource('/list/:branchId/:listId', { }, {

      // defaults: GET, QUERY, SAVE

      // postData is the list
      update: {
        url: '/list',
        method: 'PUT'
      },

      delete: {
        url: '/list/:listId',
        method: 'DELETE'
      },

      // postData is the item
      addItem: {
        url: '/list/:listId/item',
        method: 'POST'
      },

      // postData is the item
      updateItem: {
        url: '/list/:listId/item',
        method: 'PUT'
      },

      deleteItem: {
        url: '/list/:listId/item/:listItemId',
        method: 'DELETE'
      },

      // postData is an array of items
      addMultipleItems: {
        url: '/list/:listId/items',
        method: 'POST'
      },

      // postData is an array of listitemids as strings
      // deleteMultipleItems: {
      //   url: '/list/:listId/item',
      //   method: 'DELETE'
      // }

    });
  
  }]);
