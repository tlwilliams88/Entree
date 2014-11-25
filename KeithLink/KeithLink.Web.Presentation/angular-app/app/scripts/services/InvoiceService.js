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
        return Invoice.exportConfig({}).$promise;
      },

      exportInvoice: function(config) {
        ExportService.export('/invoice/export/', config);
      }

    };
 
    return Service;
 
  }]);