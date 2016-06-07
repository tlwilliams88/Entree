'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:TransactionService
 * @description
 * # TransactionService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('TransactionService', ['Invoice', 'ExportService',
    function (Invoice, ExportService) {

    var Service = {

      getPendingTransactions: function(params) {
        return Invoice.getAllPendingTransactions(params).$promise.then(function(resp){
          return resp.successResponse;
        });
      },

      getTransactionExportConfig: function(invoiceNumber) {
        return Invoice.getTransactionExportConfig({}).$promise.then(function(response){
          return response.successResponse;
        });
      },

      exportTransactions: function(config, params) {
        var exportParams = {
          paging: params.paging,
          export: config
        };
        ExportService.export('/invoice/transactions/pending/export', exportParams);
      }
    };

    return Service;

  }]);
