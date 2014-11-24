'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:InvoiceService
 * @description
 * # InvoiceService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('InvoiceService', ['Invoice', 
    function (Invoice) {
    
    var Service = {
      
      getAllInvoices: function() {
        return Invoice.query().$promise;
      },

      getInvoiceTransactions: function(invoiceNumber) {
        return Invoice.getTransactions({
          invoiceNumber: invoiceNumber
        }).$promise;
      },

      payInvoices: function(payments, account) {
        payments.forEach(function(payment) {
          payment.account = account.accountNumber;
          payment.amount = parseFloat(payment.paymentAmount);
        });

        return Invoice.pay(payments).$promise;
      },

      /********************
      EXPORT
      ********************/

      getExportConfig: function() {
        return {
          availtypes: ['csv', 'excel', 'tab'],
          selectedtype: 'tab',
          fields: [{
            field: 'description',
            label: 'Description',
            order: 2,
            selected: true
          }, {
            field: 'upc',
            label: 'UPC'
          }, {
            field: 'price',
            label: 'Price'
          }, {
            field: 'label',
            label: 'Label',
            order: 1,
            selected: true
          }]
        };
      },

      exportCustomInvoice: function(config) {
        console.log('custom export Invoice');
        console.log(config);
      },

      exportDefaultInvoice: function(config) {
        console.log('default Invoice export');
      }

    };
 
    return Service;
 
  }]);