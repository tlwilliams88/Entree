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

      validate: {
        url: '/invoice/payment/validate',
        method: 'POST'
      },

      getInvoice: {
        url: '/invoice/:invoiceNumber',
        method: 'GET'
      },

      getInvoiceTransactions: {
        url: '/invoice/transactions/:invoiceNumber',
        method: 'GET'
      },

      getInvoiceExportConfig: {
        url: '/invoice/export',
        method: 'GET'
      },

      getDetailExportConfig: {
        url: '/invoice/export/:invoiceNumber',
        method: 'GET'
      },

      getAllOpen: {
        url: '/invoice?forAllCustomers=true',
        method: 'POST'
      },

      getAllPendingTransactions: {
        url: '/invoice/transactions/pending',
        method: 'POST'
      },

      getTransactionExportConfig: {
        url: '/invoice/transactions/pending/export',
        method: 'GET'
      }

    });

  }]);
