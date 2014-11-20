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
      }
    };
 
    return Service;
 
  }]);