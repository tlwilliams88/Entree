'use strict';

angular.module('bekApp')
  .factory('Invoice', [ '$resource',
  function ($resource) {
    return $resource('/invoice', { }, {

      // defaults: GET, SAVE

      pay: {
        url: '/invoice/payment',
        method: 'POST'
      },

      getInvoice: {
        url: '/invoice/:invoiceNumber',
        method: 'GET'
      },

      getInvoiceExportConfig: {
        url: '/invoice/export',
        method: 'GET'
      },

      getDetailExportConfig: {
        url: '/invoice/export/:invoiceNumber',
        method: 'GET'
      }

    });

  }]);
