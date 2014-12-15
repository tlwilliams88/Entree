'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:InvoiceService
 * @description
 * # InvoiceService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('InvoiceService', ['ExportService', 'Invoice',
    function (ExportService, Invoice) {

    var Service = {

      getAllInvoices: function() {
        return Invoice.query().$promise;
      },

      getInvoice: function(invoiceNumber) {
        return Invoice.getInvoice({
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
        return Invoice.getInvoiceExportConfig({}).$promise;
      },

      exportInvoice: function(config) {
        ExportService.export('/invoice/export/', config);
      },

      getDetailExportConfig: function(invoiceNumber) {
        return Invoice.getDetailExportConfig({
          invoiceNumber: invoiceNumber
        }).$promise;
      },

      exportInvoiceDetails: function(config, invoiceNumber) {
        ExportService.export('/invoice/export/' + invoiceNumber, config);
      }

    };

    return Service;

  }]);
