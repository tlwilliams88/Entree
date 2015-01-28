'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:TransactionService
 * @description
 * # TransactionService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('TransactionService', ['Invoice',
    function (Invoice) {

    var Service = {

      getPendingTransactions: function(params) {
        return Invoice.getAllPendingTransactions(params).$promise;
      }
    };

    return Service;

  }]);
