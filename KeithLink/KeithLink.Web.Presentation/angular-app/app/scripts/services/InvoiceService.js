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
        });
      },

      getInvoiceImage: function(invoiceNumber, customerNumber, branchId) {
        var promise = $http.get('/invoice/image/' + invoiceNumber + '?customerid=' + customerNumber + '&branchid=' + branchId, { data: { message: 'Loading image...' } });
        return UtilityService.resolvePromise(promise);
      },

      // for all customers
      getAllCustomersWithInvoices: function(params){
        params.size = '';
        return Invoice.getAllCustomers(params).$promise.then(function(resp){
          return resp.successResponse;
        });
      },
      
      getCustomerInvoices: function(branch, customerNumber, params){
        params.size = '';

        return Invoice.getAllCustomerInvoices({ branch: branch, customerNumber: customerNumber }, params).$promise.then(function(resp){
            return resp.successResponse.pagedresults.results;
        })
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

      exportInvoice: function(exportParams) {
        var filter = '';
        if (exportParams.isViewingAllCustomers == true) {
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

      exportInvoiceDetails: function(invoiceNumber, config) {
        ExportService.export('/invoice/export/' + invoiceNumber, config);
      }

    };

    return Service;

  }]);