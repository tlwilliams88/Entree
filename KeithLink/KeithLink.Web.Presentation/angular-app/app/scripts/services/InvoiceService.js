'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:InvoiceService
 * @description
 * # InvoiceService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('InvoiceService', ['ExportService', 'Invoice', '$http', 'UtilityService',
    function (ExportService, Invoice, $http, UtilityService) {

    var Service = {

      getInvoices: function(params) {
        return Invoice.save(params).$promise;
      },

      getInvoice: function(invoiceNumber) {
        return Invoice.getInvoice({
          invoiceNumber: invoiceNumber
        }).$promise;
      },

      getInvoiceImage: function(invoiceNumber) {
        var promise = $http.get('/invoice/image/' + invoiceNumber);
        return UtilityService.resolvePromise(promise);
      },

      // for all customers
      getAllOpenInvoices: function(params){
        return Invoice.getAllOpen(params).$promise;
      },

      // for all customers
      // getPendingTransactions: function(params) {
      //   return Invoice.getAllPendingTransactions(params).$promise.then(function (data) {
      //     // add data to wrapper object so it matches format of getInvoices endpoints
      //     var wrapper = {
      //       pagedresults: data
      //     };
      //     return wrapper;
      //   });
      // },

      payInvoices: function(payments, account) {
        payments.forEach(function(payment) {
          payment.account = account.accountNumber;
          payment.amount = parseFloat(payment.paymentAmount);
        });


        return Invoice.pay({}, payments).$promise;
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
        var exportParams = {
          paging: params.paging,
          export: config
        };

        var filter = '';
        if (params.isViewingAllCustomers) {
          filter = '?forAllCustomers=true';
        }

        ExportService.export('/invoice/export/' + filter, exportParams);
      },

       
    
       setFilters: function(filterView,filterRows){       
        if(filterView){
          Service.selectedFilterView={};
          Service.selectedFilterView = filterView;
        }
        if(filterRows){
          Service.filterRowFields ={};
          Service.filterRowFields = filterRows;
        }
        return;
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
