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

      getInvoices: function(params) {
        return Invoice.save(params).$promise;
      },

      getInvoice: function(invoiceNumber) {
        return Invoice.getInvoice({
          invoiceNumber: invoiceNumber
        }).$promise;
      },

      // for all customers
      getAllOpenInvoices: function(params){
        return Invoice.getAllOpen(params).$promise;
      },

      // for all customers
      getPendingTransactions: function(params) {
        return Invoice.getAllPendingTransactions(params).$promise.then(function (data) {
          // add data to wrapper object so it matches format of getInvoices endpoints
          var wrapper = {
            pagedresults: data
          };
          return wrapper;
        });
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

      exportInvoice: function(config, params) {
        // {
        //   "paging": {"size":50,"from":0,"filter":{"filter":[],"field":"statusdescription","value":"Past Due"}},
        //   "export": {"selectedtype": "CSV"}
        // }
        params.size = null;
        params.from = null;
        var exportParams = {
          paging: params,
          export: config
        };

        ExportService.export('/invoice/export/', exportParams);
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
