'use strict';

angular.module('bekApp')
  .factory('Invoice', [ '$resource',
  function ($resource) {
    return $resource('/invoice', { }, {

      // defaults: GET, QUERY

      pay: {
        url: '/invoice/payment',
        method: 'POST'
      },

      getInvoice: {
        url: '/invoice/:invoiceNumber',
        method: 'GET'
      },

      exportConfig: {
        url: '/invoice/export',
        method: 'GET'
      }

    });

  }]);
