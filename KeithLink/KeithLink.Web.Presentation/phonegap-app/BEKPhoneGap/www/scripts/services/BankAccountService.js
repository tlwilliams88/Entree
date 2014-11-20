'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:BankAccountService
 * @description
 * # BankAccountService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('BankAccountService', [ 'BankAccount', function (BankAccount) {

  var Service = {
  
    getAllBankAccounts: function() {
      return BankAccount.query({}).$promise;
    },

    getBankAccountByAccountNumber: function() {
      return BankAccount.get({}).$promise;
    }

  };

    return Service;

  }]);
