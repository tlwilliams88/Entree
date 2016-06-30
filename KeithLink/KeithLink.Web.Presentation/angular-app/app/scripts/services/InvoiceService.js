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
        return Invoice.save(params).$promise.then(function(resp){
          return resp.successResponse;          
        });
      },

      getInvoice: function(invoiceNumber) {
        return Invoice.getInvoice({
          invoiceNumber: invoiceNumber
        }).$promise.then(function(resp){
          return resp.successResponse;
        });
      },

      getInvoiceTransactions: function(branchId, customerNumber, invoiceNumber){
        return Invoice.getInvoiceTransactions({
          branchId: branchId,
          customerNumber: customerNumber,
          invoiceNumber: invoiceNumber
        }).$promise.then(function(resp){
          return resp.successResponse;
        })
      },

      getInvoiceImage: function(invoiceNumber) {
        var promise = $http.get('/invoice/image/' + invoiceNumber, { data: { message: 'Loading image...' } });
        return UtilityService.resolvePromise(promise);
      },

      // for all customers
      getAllOpenInvoices: function(params){
        params.size = '';
        return Invoice.getAllOpen(params).$promise.then(function(resp){
          return resp.successResponse;
        });
      },

      payInvoices: function(payments) {
        payments.forEach(function(payment) {     
        payment.amount = parseFloat(payment.paymentAmount);
      });

        return Invoice.pay({}, payments).$promise.then(function(resp){
          return resp.successResponse;
        });
      },

      checkTotals: function(payments) {
        payments.forEach(function(payment) {     
        payment.amount = parseFloat(payment.paymentAmount);
      });

        return Invoice.validate({}, payments).$promise;
      },

      /********************
      EXPORT
      ********************/

      getExportConfig: function() {
        return Invoice.getInvoiceExportConfig({}).$promise.then(function(response){
          return response.successResponse;
        });
      },

      exportInvoice: function(config, params) {
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
        }).$promise.then(function(response){
          return response.successResponse;
        });
      },

      exportInvoiceDetails: function(config, invoiceNumber) {
        ExportService.export('/invoice/export/' + invoiceNumber, config);
      }

    };

    return Service;

  }]);
