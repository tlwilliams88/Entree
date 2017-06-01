'use strict';

angular.module('bekApp')
  .factory('List', [ '$resource',
  function ($resource) {
    return $resource('/list/:listType/:listId', { }, {

      // defaults: GET, QUERY, SAVE, DELETE

      // accepts type of custom, inventoryvaluation, etc.
      getByType: {
        url: '/list/type/:type',
        method: 'GET',
        isArray: false
      },

      // postData is the list
      update: {
        url: '/list/:listType/:listId',
        method: 'PUT'
      },

      delete: {
        url: '/list/:listType/:listId',
        method: 'DELETE'
      },

      // postData is the item
      addItem: {
        url: '/list/:listId/item',
        method: 'POST'
      },

      // postData is the item
      updateItem: {
        url: '/list/:listType/:listId/item',
        method: 'PUT'
      },

      deleteItem: {
        url: '/list/item/:listItemId',
        method: 'DELETE'
      },

      deleteItemByItemNumber: {
        url: '/list/:listId/item/:itemNumber',
        method: 'DELETE'
      },

      // postData is an array of items
      addMultipleItems: {
        url: '/list/:listType/:listId/items',
        method: 'POST'
      },

      getCriticalItems: {
        url: '/list/reminders',
        method: 'GET',
        isArray: false
      },

      getRecommendedItems: {
        url: '/list/recommended',
        method: 'GET',
        isArray: false
      },

      copyList: {
        url: '/list/copy',
        method: 'POST',
        isArray: false
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
