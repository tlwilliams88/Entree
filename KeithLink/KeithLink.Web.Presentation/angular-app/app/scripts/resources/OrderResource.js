'use strict';

angular.module('bekApp')
  .factory('Order', [ '$resource', 
  function ($resource) {
    return $resource('/order/:orderNumber', { }, {

      // defaults: GET, QUERY, SAVE, DELETE

      update: {
        url: '/order',
        method: 'PUT'
      },

      getOrdersByDate: {
        url: '/order/date',
        method: 'GET',
        isArray: false
      },

      resubmitOrder: {
        url: '/order/:orderNumber/changeorder',
        method: 'POST'
      },

      getOrderHistory: {
        url: '/order/history',
        method: 'POST'
      },

      getOrderHistoryDetails: {
        url: '/order/history/:invoiceNumber',
        method: 'POST'
      },

      pollOrderHistory: {
        url: '/order/lastupdate',
        method: 'GET'
      },

      getOrderExportConfig: {
        url: '/order/export',
        method: 'GET'
      },

      getDetailExportConfig: {
        url: '/order/export/:orderNumber',
        method: 'GET'
      },

      isSubmitted: {
        url:'/order/issubmitted/:orderNumber',
        method:'GET'
      }
    });
  
  }]);
