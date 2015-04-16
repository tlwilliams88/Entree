'use strict';

angular.module('bekApp')
  .factory('List', [ '$resource', 
  function ($resource) {
    return $resource('/list/:listId', { }, {

      // defaults: GET, QUERY, SAVE, DELETE

      // postData is the list
      update: {
        url: '/list',
        method: 'PUT'
      },

      // delete: {
      //   url: '/list/:listId',
      //   method: 'DELETE'
      // },

      // postData is the item
      addItem: {
        url: '/list/:listId/item',
        method: 'POST'
      },

      // postData is the item
      updateItem: {
        url: '/list/item',
        method: 'PUT'
      },

      deleteItem: {
        url: '/list/item/:listItemId',
        method: 'DELETE'
      },

      // postData is an array of items
      addMultipleItems: {
        url: '/list/:listId/items',
        method: 'POST'
      },

      getCriticalItems: {
        url: '/list/reminders',
        method: 'GET',
        isArray: true
      },

      getRecommendedItems: {
        url: '/list/recommended',
        method: 'GET',
        isArray: true
      },

      copyList: {
        url: '/list/copy',
        method: 'POST',
        isArray: true
      },

      shareList: {
        url: '/list/share',
        method: 'POST'
      },

      exportConfig: {
        url: '/list/export/:listId',
        method: 'GET'
      }

      // postData is an array of listitemids as strings
      // NOTE $resource does not accept deletes with payloads
      // deleteMultipleItems: {
      //   url: '/list/:listId/item',
      //   method: 'DELETE'
      // }

    });
  
  }]);
