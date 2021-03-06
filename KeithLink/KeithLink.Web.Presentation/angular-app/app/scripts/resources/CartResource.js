'use strict';

angular.module('bekApp')
  .factory('Cart', [ '$resource', 
  function ($resource) {
    return $resource('/cart/:cartId', { }, {

      // defaults: GET, QUERY, SAVE

      update: {
        url: '/cart',
        method: 'PUT'
      },

      updateFromQuickAdd: {
        url: '/cart/quickadd',
        method: 'PUT'
      },

      delete: {
        url: '/cart/:cartId',
        method: 'DELETE'
      },

      addItem: {
        url: '/cart/:cartId/item',
        method: 'POST'
      },

      updateItem: {
        url: '/cart/:cartId/item',
        method: 'PUT'
      },

      deleteItem: {
        url: '/cart/:cartId/item',
        method: 'DELETE'
      },

      getShipDates: {
        url: '/order/shipdays',
        method: 'GET'
      },

      submit: {
        url: '/order/:cartId',
        method: 'POST'
      },

      setActive: {
        url: '/cart/:cartId/active',
        method: 'PUT'
      },

      quickAdd: {
        url: '/cart/quickadd',
        method: 'POST'
      },

      isSubmitted: {
        url:'/cart/issubmitted/:cartId',
        method:'GET'
      }
    });
  
  }]);
